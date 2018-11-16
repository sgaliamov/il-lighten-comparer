using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Reflection;

namespace ILLightenComparer.Emit
{
    internal sealed class ComparerBuilder
    {
        private readonly Context _context;
        private readonly MembersProvider _membersProvider;

        public ComparerBuilder(Context context, MembersProvider membersProvider)
        {
            _context = context;
            _membersProvider = membersProvider;
        }

        public IComparer Build(Type objectType)
        {
            var typeBuilder = _context.DefineType($"{objectType.FullName}.Comparer", InterfaceType.Comparer);

            var staticCompare = BuildStaticMethods(typeBuilder);

            BuildCompareMethod(typeBuilder, staticCompare);

            var typeInfo = typeBuilder.CreateTypeInfo();

            return _context.CreateInstance<IComparer>(typeInfo);
        }

        private MethodBuilder BuildStaticMethods(TypeBuilder typeBuilder)
        {
            var staticMethodBuilder = typeBuilder.DefineStaticMethod(
                Constants.CompareMethodName,
                typeof(int),
                new[] { typeof(object), typeof(object), typeof(object) });
            EmitReferenceEquals(staticMethodBuilder.GetILGenerator());

            var members = _membersProvider.GetMembers(typeBuilder, _context.Configuration);
            foreach (var member in members)
            {
                
            }

            typeBuilder.BuildFactoryMethod<IComparer>();

            return staticMethodBuilder;
        }

        private static void EmitReferenceEquals(ILGenerator il)
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

            il.Emit(OpCodes.Ldc_I4_8);
            il.Emit(OpCodes.Ret);
        }

        private static void BuildCompareMethod(TypeBuilder typeBuilder, MethodInfo staticCompareMethod)
        {
            var methodBuilder = typeBuilder.DefineInterfaceMethod(Method.Compare);

            var il = methodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Call, staticCompareMethod);
            il.Emit(OpCodes.Ret);
        }
    }
}
