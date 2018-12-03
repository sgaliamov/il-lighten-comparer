using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Members;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit
{
    internal sealed class ComparerTypeBuilder
    {
        private readonly CompareEmitter _compareEmitter;
        private readonly Context _context;
        private readonly MembersProvider _membersProvider;

        public ComparerTypeBuilder(Context context, MembersProvider membersProvider)
        {
            _context = context;
            _membersProvider = membersProvider;
            _compareEmitter = new CompareEmitter(_context);
        }

        public TypeInfo Build(Type objectType)
        {
            var basicInterface = typeof(IComparer);
            var genericInterface = typeof(IComparer<>).MakeGenericType(objectType);

            var typeBuilder = _context.DefineType(
                $"{objectType.FullName}.DynamicComparer",
                basicInterface,
                genericInterface
            );

            var contextField = typeBuilder.DefineField(
                "_context",
                typeof(IContext),
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
                contextField);

            return typeBuilder.CreateTypeInfo();
        }

        

        private static void BuildFactory(TypeBuilder typeBuilder, FieldInfo contextField)
        {
            var parameters = new[] { typeof(IContext) };

            var constructorInfo = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.HasThis,
                parameters);

            using (var il = constructorInfo.CreateILEmitter())
            {
                il.LoadArgument(0)
                  .Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes))
                  .LoadArgument(0)
                  .LoadArgument(1)
                  .Emit(OpCodes.Stfld, contextField)
                  .Emit(OpCodes.Ret);
            }

            var methodBuilder = typeBuilder.DefineStaticMethod(
                MethodName.Factory,
                typeBuilder,
                parameters);

            using (var il = methodBuilder.CreateILEmitter())
            {
                il.LoadArgument(0)
                  .EmitCtorCall(constructorInfo);
            }
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
                    EmitReferenceComparision(il);
                }

                EmitMembersComparision(il, objectType);
            }

            return staticMethodBuilder;
        }

        private void EmitMembersComparision(ILEmitter il, Type objectType)
        {
            var members = _membersProvider.GetMembers(objectType);

            InitFirstLocalToKeepComparisonsResult(il);
            foreach (var member in members)
            {
                member.Accept(_compareEmitter, il);
            }

            EmitDefaultResult(il);
        }

        private static void InitFirstLocalToKeepComparisonsResult(ILEmitter il)
        {
            il.DeclareLocal(typeof(int), 0, out _);
        }

        private static void EmitDefaultResult(ILEmitter il)
        {
            il.Return(0);
        }

        private static void EmitReferenceComparision(ILEmitter il)
        {
            // x == y
            il.LoadArgument(1)
              .LoadArgument(2)
              .Branch(OpCodes.Bne_Un_S, out var checkY)
              .Return(0)
              .MarkLabel(checkY)
              // y != null
              .LoadArgument(2)
              .Branch(OpCodes.Brtrue_S, out var checkX)
              .Return(1)
              .MarkLabel(checkX)
              // x != null
              .LoadArgument(1)
              .Branch(OpCodes.Brtrue_S, out var next)
              .Return(-1)
              .MarkLabel(next);
        }

        private static void BuildBasicCompareMethod(
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
                    EmitReferenceComparision(il);
                }

                il.LoadArgument(0)
                  .Emit(OpCodes.Ldfld, contextField)
                  .LoadArgument(1)
                  .EmitCast(objectType)
                  .LoadArgument(2)
                  .EmitCast(objectType)
                  .Emit(OpCodes.Newobj, Method.HashSetConstructor)
                  .Call(staticCompareMethod)
                  .Emit(OpCodes.Ret);
            }
        }

        private static void BuildTypedCompareMethod(
            TypeBuilder typeBuilder,
            MethodInfo interfaceMethod,
            MethodInfo staticCompareMethod,
            FieldInfo contextField)
        {
            var methodBuilder = typeBuilder.DefineInterfaceMethod(interfaceMethod);

            using (var il = methodBuilder.CreateILEmitter())
            {
                il.LoadArgument(0)
                  .Emit(OpCodes.Ldfld, contextField)
                  .LoadArgument(1)
                  .LoadArgument(2)
                  .Emit(OpCodes.Newobj, Method.HashSetConstructor)
                  .Call(staticCompareMethod)
                  .Emit(OpCodes.Ret);
            }
        }
    }
}
