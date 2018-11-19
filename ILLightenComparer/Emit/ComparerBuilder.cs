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
    internal sealed class ComparerBuilder
    {
        private readonly Context _context;
        private readonly CompareEmitVisitor _emitVisitor = new CompareEmitVisitor();
        private readonly MembersProvider _membersProvider;

        public ComparerBuilder(Context context, MembersProvider membersProvider)
        {
            _context = context;
            _membersProvider = membersProvider;
        }

        public IComparer Build(Type objectType)
        {
            var genericInterfaceType = InterfaceType.GenericComparer.MakeGenericType(objectType);

            var typeBuilder = _context.DefineType(
                $"{objectType.FullName}.Comparer",
                InterfaceType.Comparer,
                genericInterfaceType
            );

            var staticCompare = BuildStaticCompareMethod(typeBuilder, objectType);

            BuildInstanceCompareMethod(typeBuilder, staticCompare, objectType, genericInterfaceType);

            typeBuilder.BuildFactoryMethod<IComparer>();
            var typeInfo = typeBuilder.CreateTypeInfo();

            return _context.CreateInstance<IComparer>(typeInfo);
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
            EmitMembersComparision(objectType, il);
            EmitDefaultResult(il);

            return staticMethodBuilder;
        }

        private void EmitMembersComparision(Type objectType, ILGenerator il)
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

        private static void BuildInstanceCompareMethod(
            TypeBuilder typeBuilder,
            MethodInfo staticCompareMethod,
            Type objectType,
            Type genericInterfaceType)
        {
            var cast = objectType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass;

            var methodBuilder = typeBuilder.DefineInterfaceMethod(Method.Compare);

            var il = methodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldc_I4_0); // todo: hash set to detect cycles
            il.Emit(OpCodes.Ldarg_1); // x
            il.Emit(cast, objectType);
            il.Emit(OpCodes.Ldarg_2); // y
            il.Emit(cast, objectType);
            il.Emit(OpCodes.Call, staticCompareMethod);
            il.Emit(OpCodes.Ret);

            methodBuilder = typeBuilder.DefineInterfaceMethod(genericInterfaceType.GetMethod(Constants.CompareMethodName));

            il = methodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldc_I4_0); // todo: hash set to detect cycles
            il.Emit(OpCodes.Ldarg_1); // x
            il.Emit(OpCodes.Ldarg_2); // y
            il.Emit(OpCodes.Call, staticCompareMethod);
            il.Emit(OpCodes.Ret);
        }
    }
}
