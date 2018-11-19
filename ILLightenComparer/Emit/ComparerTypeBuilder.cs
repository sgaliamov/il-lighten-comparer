using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Visitors;
using ILLightenComparer.Reflection;

namespace ILLightenComparer.Emit
{
    internal sealed class ComparerTypeBuilder
    {
        private readonly Context _context;
        private readonly CompareEmitVisitor _emitVisitor = new CompareEmitVisitor();
        private readonly MembersProvider _membersProvider;

        public ComparerTypeBuilder(Context context, MembersProvider membersProvider)
        {
            _context = context;
            _membersProvider = membersProvider;
        }

        public TypeInfo Build(Type objectType)
        {
            var genericInterface = typeof(IComparer<>).MakeGenericType(objectType);
            var regularInterface = typeof(IComparer);

            var typeBuilder = _context.DefineType(
                $"{objectType.FullName}.Comparer",
                regularInterface,
                genericInterface
            );

            var staticCompare = BuildStaticCompareMethod(typeBuilder, objectType);

            BuildRegularCompareMethod(
                typeBuilder,
                staticCompare,
                regularInterface.GetMethod(Constants.CompareMethodName),
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

            var il = staticMethodBuilder.GetILGenerator();
            if (objectType.IsClass)
            {
                EmitReferenceComparision(il);
            }

            EmitMembersComparision(il, objectType);
            EmitDefaultResult(il);

            return staticMethodBuilder;
        }

        private void EmitMembersComparision(ILGenerator il, Type objectType)
        {
            var members = _membersProvider.GetMembers(objectType, _context.Configuration);

            il.DeclareLocal(typeof(int)); // to store comparison result. todo: reuse locals.

            foreach (var memberInfo in members)
            {
                switch (memberInfo)
                {
                    case FieldInfo field:
                        _emitVisitor.Visit(il, field);
                        break;

                    case PropertyInfo property:
                        _emitVisitor.Visit(il, property);
                        break;

                    default:
                        throw new NotSupportedException(
                            "Only fields and properties are supported. "
                            + $"{memberInfo.MemberType}: {memberInfo.DisplayName()}");
                }
            }
        }

        private static void EmitDefaultResult(ILGenerator il)
        {
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Ret);
        }

        private static void EmitReferenceComparision(ILGenerator il)
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

        private static void BuildRegularCompareMethod(
            TypeBuilder typeBuilder,
            MethodInfo staticCompareMethod,
            MethodInfo interfaceMethod,
            Type objectType)
        {
            var castOp = objectType.IsValueType ? OpCodes.Unbox : OpCodes.Castclass;
            var methodBuilder = typeBuilder.DefineInterfaceMethod(interfaceMethod);

            var il = methodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldc_I4_0); // todo: hash set to detect cycles
            il.Emit(OpCodes.Ldarg_1); // x
            il.Emit(castOp, objectType);
            il.Emit(OpCodes.Ldarg_2); // y
            il.Emit(castOp, objectType);
            il.Emit(OpCodes.Call, staticCompareMethod);
            il.Emit(OpCodes.Ret);
        }

        private static void BuildTypedCompareMethod(
            TypeBuilder typeBuilder,
            MethodInfo staticCompareMethod,
            MethodInfo interfaceMethod)
        {
            var methodBuilder = typeBuilder.DefineInterfaceMethod(interfaceMethod);

            var il = methodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldc_I4_0); // todo: hash set to detect cycles
            il.Emit(OpCodes.Ldarg_1); // x
            il.Emit(OpCodes.Ldarg_2); // y
            il.Emit(OpCodes.Call, staticCompareMethod);
            il.Emit(OpCodes.Ret);
        }
    }
}
