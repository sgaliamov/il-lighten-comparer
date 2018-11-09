using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Reflection;

namespace ILLightenComparer.Emit.Methods
{
    internal sealed class CompareMethodEmitter
    {
        private readonly MembersProvider _membersProvider = new MembersProvider();

        public void Emit(Type objectType, CompareConfiguration configuration, MethodBuilder method)
        {
            var il = method.GetILGenerator();
            
            EmitReferenceEquals(il);
            return;

            var members = _membersProvider.GetMembers(objectType, configuration);
            foreach (var member in members)
            {
                if (IsIntegralType(member))
                {
                    EmitIntegralTypeComparison(member, il);
                }
                else if (IsComparableType(member))
                {
                    EmitComparableTypeComparison(member, il);
                }

                throw new NotSupportedException(
                    $"Can't emit comparison for {member.DeclaringType}.{member.Name}");
            }
        }

        private void EmitComparableTypeComparison(MemberInfo member, ILGenerator il)
        {
            throw new NotImplementedException();
        }

        private void EmitIntegralTypeComparison(MemberInfo member, ILGenerator il)
        {
            throw new NotImplementedException();
        }

        private bool IsComparableType(MemberInfo member) => throw new NotImplementedException();

        private bool IsIntegralType(MemberInfo member)
        {
            return true;
        }

        private static void EmitReferenceEquals(ILGenerator il)
        {
            // x == y
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            var checkY = il.DefineLabel();
            il.Emit(OpCodes.Bne_Un_S, checkY);
            il.Emit(OpCodes.Ldc_I4_0); // return 0
            il.Emit(OpCodes.Ret);

            // y != null
            il.MarkLabel(checkY);
            il.Emit(OpCodes.Ldarg_2);
            var checkX = il.DefineLabel();
            il.Emit(OpCodes.Brtrue_S, checkX);
            il.Emit(OpCodes.Ldc_I4_1); // return 1
            il.Emit(OpCodes.Ret);

            // x != null
            il.MarkLabel(checkX);
            il.Emit(OpCodes.Ldarg_1);
            var rest = il.DefineLabel();
            il.Emit(OpCodes.Brtrue_S, rest);
            il.Emit(OpCodes.Ldc_I4_M1); // return -1
            il.Emit(OpCodes.Ret);

            il.MarkLabel(rest);
        }
    }
}
