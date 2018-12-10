using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;
using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit
{
    internal sealed class ComparerTypeBuilder
    {
        private readonly CompareEmitter _compareEmitter;
        private readonly ComparerContext _context;
        private readonly MembersProvider _membersProvider;

        public ComparerTypeBuilder(ComparerContext context, MembersProvider membersProvider)
        {
            _context = context;
            _membersProvider = membersProvider;
            _compareEmitter = new CompareEmitter(_context);
        }

        public Type Build(Type objectType)
        {
            if (objectType.GetUnderlyingType().IsPrimitive())
            {
                throw new NotSupportedException(
                    $"Generation a comparer for primitive type {objectType.FullName} is not supported.");
            }

            var basicInterface = typeof(IComparer);
            var genericInterface = typeof(IComparer<>).MakeGenericType(objectType);

            var typeBuilder = _context.DefineType(
                $"{objectType.FullName}.DynamicComparer",
                basicInterface,
                genericInterface
            );

            var contextField = typeBuilder.DefineField(
                "_context",
                typeof(IComparerContext),
                FieldAttributes.InitOnly | FieldAttributes.Private);

            BuildFactory(typeBuilder, contextField);

            var staticCompare = BuildStaticCompareMethod(typeBuilder, objectType);

            BuildBasicCompareMethod(
                typeBuilder,
                basicInterface.GetMethod(MethodName.Compare),
                staticCompare,
                contextField,
                objectType);

            BuildTypedCompareMethod(
                typeBuilder,
                genericInterface.GetMethod(MethodName.Compare),
                staticCompare,
                contextField,
                objectType);

            return typeBuilder.CreateTypeInfo();
        }

        private MethodBuilder BuildStaticCompareMethod(TypeBuilder typeBuilder, Type objectType)
        {
            var staticMethodBuilder = typeBuilder.DefineStaticMethod(
                MethodName.Compare,
                typeof(int),
                Method.StaticCompareMethodParameters(objectType));

            using (var il = staticMethodBuilder.CreateILEmitter())
            {
                if (objectType.IsClass)
                {
                    _compareEmitter.EmitReferenceComparison(il);
                }

                if (IsDetectCyclesEnabled(objectType))
                {
                    EmitCycleDetection(il);
                }

                EmitMembersComparison(il, objectType);
            }

            return staticMethodBuilder;
        }

        private void BuildBasicCompareMethod(
            TypeBuilder typeBuilder,
            MethodInfo interfaceMethod,
            MethodInfo staticCompareMethod,
            FieldInfo contextField,
            Type objectType)
        {
            var methodBuilder = typeBuilder.DefineInterfaceMethod(interfaceMethod);

            using (var il = methodBuilder.CreateILEmitter())
            {
                if (objectType.IsValueType)
                {
                    _compareEmitter.EmitReferenceComparison(il);
                }

                il.LoadArgument(Arg.This)
                  .Emit(OpCodes.Ldfld, contextField)
                  .LoadArgument(Arg.X)
                  .EmitCast(objectType)
                  .LoadArgument(Arg.Y)
                  .EmitCast(objectType);

                EmitStaticCompareMethod(il, staticCompareMethod, objectType);
            }
        }

        private void BuildTypedCompareMethod(
            TypeBuilder typeBuilder,
            MethodInfo interfaceMethod,
            MethodInfo staticCompareMethod,
            FieldInfo contextField,
            Type objectType)
        {
            var methodBuilder = typeBuilder.DefineInterfaceMethod(interfaceMethod);

            using (var il = methodBuilder.CreateILEmitter())
            {
                il.LoadArgument(Arg.This)
                  .Emit(OpCodes.Ldfld, contextField)
                  .LoadArgument(Arg.X)
                  .LoadArgument(Arg.Y);

                EmitStaticCompareMethod(il, staticCompareMethod, objectType);
            }
        }

        private void EmitMembersComparison(ILEmitter il, Type objectType)
        {
            var members = _membersProvider.GetMembers(objectType);

            InitFirstLocalToKeepComparisonsResult(il);
            foreach (var member in members)
            {
                member.Accept(_compareEmitter, il);
            }

            il.Return(0);
        }

        private void EmitStaticCompareMethod(ILEmitter il, MethodInfo staticCompareMethod, Type objectType)
        {
            if (!_context.GetConfiguration(objectType).DetectCycles)
            {
                il.Emit(OpCodes.Ldnull)
                  .Emit(OpCodes.Ldnull)
                  .Call(staticCompareMethod)
                  .Return();

                return;
            }

            il.Emit(OpCodes.Newobj, Method.SetConstructor)
              .Store(typeof(ObjectsSet), 0, out var xSet)
              .Emit(OpCodes.Newobj, Method.SetConstructor)
              .Store(typeof(ObjectsSet), 1, out var ySet)
              .LoadLocal(xSet)
              .LoadLocal(ySet)
              .Call(staticCompareMethod)
              // if (compare != 0) return compare;
              .Store(typeof(int), out var result)
              .LoadLocal(result)
              .Branch(OpCodes.Brfalse_S, out var setsDiff)
              .LoadLocal(result)
              .Return()
              .MarkLabel(setsDiff)
              // else: return setX.Count - setY.Count;
              .LoadLocal(xSet)
              .Emit(OpCodes.Call, Method.SetGetCount)
              .LoadLocal(ySet)
              .Emit(OpCodes.Call, Method.SetGetCount)
              .Emit(OpCodes.Sub)
              .Return();
        }

        private bool IsDetectCyclesEnabled(Type objectType) =>
            objectType.IsClass && _context.GetConfiguration(objectType).DetectCycles;

        private static void EmitCycleDetection(ILEmitter il)
        {
            il.LoadArgument(Arg.SetX)
              .LoadArgument(Arg.X)
              .LoadConstant(0)
              .Emit(OpCodes.Call, Method.SetAdd)
              .LoadArgument(Arg.SetY)
              .LoadArgument(Arg.Y)
              .LoadConstant(0)
              .Emit(OpCodes.Call, Method.SetAdd)
              .Emit(OpCodes.Or)
              .LoadConstant(0)
              .Emit(OpCodes.Ceq)
              .Branch(OpCodes.Brfalse_S, out var next)
              .Return(0)
              .MarkLabel(next);
        }

        private static void BuildFactory(TypeBuilder typeBuilder, FieldInfo contextField)
        {
            var parameters = new[] { typeof(IComparerContext) };

            var constructorInfo = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.HasThis,
                parameters);

            using (var il = constructorInfo.CreateILEmitter())
            {
                il.LoadArgument(Arg.This)
                  .Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes))
                  .LoadArgument(Arg.This)
                  .LoadArgument(1)
                  .Emit(OpCodes.Stfld, contextField)
                  .Return();
            }

            var methodBuilder = typeBuilder.DefineStaticMethod(
                MethodName.Factory,
                typeBuilder,
                parameters);

            using (var il = methodBuilder.CreateILEmitter())
            {
                il.LoadArgument(Arg.This)
                  .Emit(OpCodes.Newobj, constructorInfo)
                  .Return();
            }
        }

        private static void InitFirstLocalToKeepComparisonsResult(ILEmitter il)
        {
            il.DeclareLocal(typeof(int), 0, out _);
        }
    }
}
