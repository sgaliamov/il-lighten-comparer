using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Members;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Members.Comparable;
using ILLightenComparer.Emit.Members.Integral;

namespace ILLightenComparer.Emit.Emitters
{
    internal sealed class CompareEmitter
    {
        private readonly StackEmitter _stackEmitter;

        public CompareEmitter(TypeBuilderContext context) => _stackEmitter = new StackEmitter(context);

        public ILEmitter Visit(IComparableMember member, ILEmitter il) =>
            member.Accept(_stackEmitter, il)
                  .Emit(OpCodes.Call, member.CompareToMethod)
                  .EmitReturnNotZero();

        public ILEmitter Visit(IIntegralMember member, ILEmitter il) =>
            member.Accept(_stackEmitter, il)
                  .Emit(OpCodes.Sub)
                  .EmitReturnNotZero();

        public ILEmitter Visit(NullablePropertyMember member, ILEmitter il)
        {
            member.Accept(_stackEmitter, il)
                  .DefineLabel(out var next);

            LoadValuesFromNullable(il, member, next)
                .Emit(OpCodes.Call, member.CompareToMethod)
                .EmitReturnNotZero(next);

            return il;
        }

        private static ILEmitter LoadValuesFromNullable(
            ILEmitter il,
            INullableMember member,
            Label next)
        {
            var propertyType = member.MemberType;

            return il.DeclareLocal(propertyType, out var n1, 0)
                     .DeclareLocal(propertyType, out var n2, 1)
                     .Store(n2)
                     .LoadAddress(n2)
                     // var secondHasValue = n2->HasValue
                     .Call(propertyType, member.HasValueMethod)
                     .DeclareLocal(typeof(bool), out var secondHasValue)
                     .Store(secondHasValue)
                     // var n1 = &arg1
                     .Store(n1)
                     .LoadAddress(n1)
                     // if n1->HasValue goto firstHasValue
                     .Call(propertyType, member.HasValueMethod)
                     .Branch(OpCodes.Brtrue_S, out var firstHasValue)
                     // if n2->HasValue goto returnZero
                     .LoadLocal(secondHasValue)
                     .Emit(OpCodes.Brfalse_S, next)
                     // else return -1
                     .Emit(OpCodes.Ldc_I4_M1)
                     .Emit(OpCodes.Ret)
                     // firstHasValue:
                     .MarkLabel(firstHasValue)
                     .LoadLocal(secondHasValue)
                     .Branch(OpCodes.Brtrue_S, out var getValues)
                     // return 1
                     .Emit(OpCodes.Ldc_I4_1)
                     .Emit(OpCodes.Ret)
                     // getValues: load values
                     .MarkLabel(getValues)
                     .LoadAddress(n1)
                     .Call(propertyType, member.GetValueMethod)
                     .DeclareLocal(member.MemberType.GetUnderlyingType(), out var local)
                     .Store(local)
                     .LoadAddress(local)
                     .LoadAddress(n2)
                     .Call(propertyType, member.GetValueMethod);
        }
    }
}
