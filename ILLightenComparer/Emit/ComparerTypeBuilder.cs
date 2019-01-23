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
        private readonly Converter _converter;
        private readonly MembersProvider _membersProvider;

        public ComparerTypeBuilder(ComparerContext context)
        {
            _context = context;
            _converter = new Converter();
            _compareEmitter = new CompareEmitter(context, _converter);
            _membersProvider = new MembersProvider(context, _converter);
        }

        public Type Build(TypeBuilder comparerTypeBuilder, MethodBuilder staticCompareBuilder, Type objectType)
        {
            var contextField = comparerTypeBuilder.DefineField(
                "_context",
                typeof(IComparerContext),
                FieldAttributes.InitOnly | FieldAttributes.Private);

            BuildConstructorAndFactoryMethod(comparerTypeBuilder, contextField);

            BuildStaticCompareMethod(objectType, staticCompareBuilder);

            BuildBasicCompareMethod(
                comparerTypeBuilder,
                staticCompareBuilder,
                contextField,
                objectType);

            BuildTypedCompareMethod(
                comparerTypeBuilder,
                staticCompareBuilder,
                contextField,
                objectType);

            return comparerTypeBuilder.CreateTypeInfo();
        }

        private void BuildStaticCompareMethod(Type objectType, MethodBuilder staticMethodBuilder)
        {
            using (var il = staticMethodBuilder.CreateILEmitter())
            {
                // todo: use argument variable
                if (IsEmitReferenceComparison(objectType))
                {
                    _compareEmitter.EmitArgumentsReferenceComparison(il);
                }

                if (IsDetectCycles(objectType))
                {
                    EmitCycleDetection(il);
                }

                EmitComparison(il, objectType);
            }
        }

        private void BuildBasicCompareMethod(
            TypeBuilder typeBuilder,
            MethodInfo staticCompareMethod,
            FieldInfo contextField,
            Type objectType)
        {
            var interfaceMethod = typeof(IComparer).GetMethod(MethodName.Compare);
            var methodBuilder = typeBuilder.DefineInterfaceMethod(interfaceMethod);

            using (var il = methodBuilder.CreateILEmitter())
            {
                if (objectType.IsValueType)
                {
                    _compareEmitter.EmitArgumentsReferenceComparison(il);
                }

                il.LoadArgument(Arg.This)
                  .Emit(OpCodes.Ldfld, contextField)
                  .LoadArgument(Arg.X)
                  .EmitCast(objectType)
                  .LoadArgument(Arg.Y)
                  .EmitCast(objectType);

                EmitStaticCompareMethodCall(il, staticCompareMethod, objectType);
            }
        }

        private void BuildTypedCompareMethod(
            TypeBuilder typeBuilder,
            MethodInfo staticCompareMethod,
            FieldInfo contextField,
            Type objectType)
        {
            var genericInterface = typeof(IComparer<>).MakeGenericType(objectType);
            var interfaceMethod = genericInterface.GetMethod(MethodName.Compare);
            var methodBuilder = typeBuilder.DefineInterfaceMethod(interfaceMethod);

            using (var il = methodBuilder.CreateILEmitter())
            {
                il.LoadArgument(Arg.This)
                  .Emit(OpCodes.Ldfld, contextField)
                  .LoadArgument(Arg.X)
                  .LoadArgument(Arg.Y);

                EmitStaticCompareMethodCall(il, staticCompareMethod, objectType);
            }
        }

        private void EmitComparison(ILEmitter il, Type objectType)
        {
            InitFirstLocalToKeepComparisonsResult(il);

            var argumentComparison = _converter.CreateArgumentComparison(objectType);
            if (argumentComparison == null)
            {
                EmitMembersComparison(il, objectType);
            }
            else
            {
                argumentComparison.Accept(_compareEmitter, il);
            }

            il.Return(0);
        }

        private void EmitMembersComparison(ILEmitter il, Type objectType)
        {
            if (objectType.GetUnderlyingType().IsPrimitive())
            {
                throw new InvalidOperationException($"{objectType.DisplayName()} is not expected.");
            }

            var members = _membersProvider.GetMembers(objectType);
            foreach (var member in members)
            {
                member.Accept(_compareEmitter, il);
            }
        }

        private void EmitStaticCompareMethodCall(ILEmitter il, MethodInfo staticCompareMethod, Type objectType)
        {
            if (!IsCreateCycleDetectionSets(objectType))
            {
                il.Emit(OpCodes.Ldnull)
                  .Emit(OpCodes.Ldnull)
                  .Call(staticCompareMethod)
                  .Return();

                return;
            }

            il.Emit(OpCodes.Newobj, Method.SetConstructor)
              .Emit(OpCodes.Newobj, Method.SetConstructor)
              .Call(staticCompareMethod)
              .Return();
        }

        private bool IsCreateCycleDetectionSets(Type objectType)
        {
            return _context.GetConfiguration(objectType).DetectCycles
                   && !objectType.GetUnderlyingType().IsPrimitive()
                   && !objectType.IsCollectionOfSealed()
                   && !objectType.IsSealedComparable();
        }

        private bool IsDetectCycles(Type objectType)
        {
            return objectType.IsClass && IsCreateCycleDetectionSets(objectType);
        }

        private static bool IsEmitReferenceComparison(Type objectType)
        {
            return objectType.IsClass && !objectType.IsCollectionOfSealed();
        }

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
              // else: return setX.Count - setY.Count;
              .LoadArgument(Arg.SetX)
              .Emit(OpCodes.Call, Method.SetGetCount)
              .LoadArgument(Arg.SetY)
              .Emit(OpCodes.Call, Method.SetGetCount)
              .Emit(OpCodes.Sub)
              .Return()
              .MarkLabel(next);
        }

        private static void BuildConstructorAndFactoryMethod(TypeBuilder typeBuilder, FieldInfo contextField)
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
                MethodName.CreateInstance,
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
