using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Reflection;

namespace ILLightenComparer.Emit.Methods
{
    internal sealed class CompareMethodEmitter
    {
        private readonly Context _context;
        private readonly MembersProvider _membersProvider = new MembersProvider();

        public CompareMethodEmitter(Context context) => _context = context;

        public MethodInfo Emit(Type objectType)
        {
            var method = _context.DefineStaticMethod(
                $"{objectType.FullName}.Compare",
                typeof(int),
                new[] { objectType, objectType });

            var il = method.GetILGenerator();

            EmitReferenceEquals(il);

            var members = _membersProvider.GetMembers(objectType, _context.Configuration);
            foreach (var member in members)
            {
                if (IsIntegralType(member))
                {
                    EmitIntegralTypeComparison(member, il);
                    continue;
                }

                if (IsComparableType(member))
                {
                    EmitComparableTypeComparison(member, il);
                    continue;
                }

                throw new NotSupportedException(
                    $"Can't emit comparison for {member.DeclaringType}.{member.Name}");
            }

            il.Emit(OpCodes.Ldc_I4_0); // return 0
            il.Emit(OpCodes.Ret);

            return method;
        }

        private void EmitComparableTypeComparison(MemberInfo member, ILGenerator il) { }

        private void EmitIntegralTypeComparison(MemberInfo member, ILGenerator il) { }

        private bool IsComparableType(MemberInfo member) => true;

        private bool IsIntegralType(MemberInfo member) => true;

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
        }
    }
}
