using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.Visitors;

namespace ILLightenComparer.Emit
{
    internal sealed class ComparerTypeBuilder
    {
        private readonly TypeBuilderContext _context;
        private readonly CompareEmitVisitor _emitVisitor = new CompareEmitVisitor();
        private readonly MembersProvider _membersProvider;

        public ComparerTypeBuilder(TypeBuilderContext context, MembersProvider membersProvider)
        {
            _context = context;
            _membersProvider = membersProvider;
        }

        public TypeInfo Build(Type objectType)
        {
            var basicInterface = typeof(IComparer);
            var genericInterface = typeof(IComparer<>).MakeGenericType(objectType);

            var typeBuilder = _context.DefineType(
                $"{objectType.FullName}.Comparer",
                basicInterface,
                genericInterface
            );

            var staticCompare = BuildStaticCompareMethod(typeBuilder, objectType);

            BuildBasicCompareMethod(
                typeBuilder,
                staticCompare,
                basicInterface.GetMethod(Constants.CompareMethodName),
                objectType);

            BuildTypedCompareMethod(
                typeBuilder,
                staticCompare,
                genericInterface.GetMethod(Constants.CompareMethodName));

            return typeBuilder
                   .BuildFactoryMethod()
                   .CreateTypeInfo();
        }

        private MethodBuilder BuildStaticCompareMethod(TypeBuilder typeBuilder, Type objectType)
        {
            var staticMethodBuilder = typeBuilder.DefineStaticMethod(
                Constants.CompareMethodName,
                typeof(int),
                new[]
                {
                    typeof(HashSet<object>),
                    objectType, // x
                    objectType // y
                });

            var il = staticMethodBuilder.GetILEmitter();

            if (objectType.IsClass)
            {
                EmitReferenceComparision(il);
            }

            EmitMembersComparision(il, objectType);
            EmitDefaultResult(il);

            return staticMethodBuilder;
        }

        private void EmitMembersComparision(ILEmitter il, Type objectType)
        {
            var members = _membersProvider.GetMembers(objectType, _context.Configuration);

            foreach (var member in members)
            {
                member.Accept(_emitVisitor, il);
            }
        }

        private static void EmitDefaultResult(ILEmitter il)
        {
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Ret);
        }

        private static void EmitReferenceComparision(ILEmitter il)
        {
            // x == y
            var else0 = il.DefineLabel();
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Bne_Un_S, else0);
            il.Emit(OpCodes.Ldc_I4_0); // return 0
            il.Emit(OpCodes.Ret);
            il.MarkLabel(else0);

            // y != null
            var else1 = il.DefineLabel();
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Brtrue_S, else1);
            il.Emit(OpCodes.Ldc_I4_1); // return 1
            il.Emit(OpCodes.Ret);
            il.MarkLabel(else1);

            // x != null
            var else2 = il.DefineLabel();
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Brtrue_S, else2);
            il.Emit(OpCodes.Ldc_I4_M1); // return -1
            il.Emit(OpCodes.Ret);
            il.MarkLabel(else2);
        }

        private static void BuildBasicCompareMethod(
            TypeBuilder typeBuilder,
            MethodInfo staticCompareMethod,
            MethodInfo interfaceMethod,
            Type objectType)
        {
            var methodBuilder = typeBuilder.DefineInterfaceMethod(interfaceMethod);
            var il = methodBuilder.GetILEmitter();

            if (objectType.IsValueType)
            {
                EmitReferenceComparision(il);
            }

            // todo: hash set to detect cycles
            il.Emit(OpCodes.Ldc_I4_0)
              .Emit(OpCodes.Ldarg_1) // x
              .EmitCast(objectType)
              .Emit(OpCodes.Ldarg_2) // y
              .EmitCast(objectType)
              .Emit(OpCodes.Call, staticCompareMethod)
              .Emit(OpCodes.Ret);
        }

        private static void BuildTypedCompareMethod(
            TypeBuilder typeBuilder,
            MethodInfo staticCompareMethod,
            MethodInfo interfaceMethod)
        {
            typeBuilder
                .DefineInterfaceMethod(interfaceMethod)
                .GetILEmitter()
                .Emit(OpCodes.Ldc_I4_0) // todo: hash set to detect cycles
                .Emit(OpCodes.Ldarg_1) // x
                .Emit(OpCodes.Ldarg_2)
                .Emit(OpCodes.Call, staticCompareMethod)
                .Emit(OpCodes.Ret);
        }
    }
}
