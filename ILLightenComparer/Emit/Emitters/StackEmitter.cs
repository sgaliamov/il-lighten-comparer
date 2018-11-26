using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Members.Comparable;
using ILLightenComparer.Emit.Members.Integral;

namespace ILLightenComparer.Emit.Emitters
{
    internal sealed class StackEmitter
    {
        private readonly TypeBuilderContext _context;

        public StackEmitter(TypeBuilderContext context) => _context = context;

        public void Visit(ComparableFieldMember member, ILEmitter il)
        {
            il.LoadFieldAddress(member, 1)
              .LoadField(member, 2);
        }

        public void Visit(ComparablePropertyMember member, ILEmitter il)
        {
            il.LoadPropertyAddress(member, 1)
              .LoadProperty(member, 2);
        }

        public void Visit(StringFiledMember member, ILEmitter il)
        {
            var comparisonType = (int)_context.Configuration.StringComparisonType;

            il.LoadField(member, 1)
              .LoadField(member, 2)
              .Emit(OpCodes.Ldc_I4_S, comparisonType); // todo: use short form
        }

        public void Visit(StringPropertyMember member, ILEmitter il)
        {
            var comparisonType = (int)_context.Configuration.StringComparisonType;

            il.LoadProperty(member, 1)
              .LoadProperty(member, 2)
              .Emit(OpCodes.Ldc_I4_S, comparisonType); // todo: use short form
        }

        public void Visit(IntegralFiledMember member, ILEmitter il)
        {
            il.LoadField(member, 1)
              .LoadField(member, 2);
        }

        public void Visit(IntegralPropertyMember member, ILEmitter il)
        {
            il.LoadProperty(member, 1)
              .LoadProperty(member, 2);
        }

        public void Visit(NullablePropertyMember member, ILEmitter il)
        {
            il.LoadPropertyAddress(member, 2) // var n2 = arg2 
              // var secondHasValue = n2.HasValue
              .Call(member, member.HasValueMethod)
              .TempLocal(typeof(bool), out var secondHasValue)
              .Store(secondHasValue)
              // n1 = arg1
              .LoadPropertyAddress(member, 1)
              .Call(member, member.HasValueMethod)
              // if n1.HasValue goto firstHasValue
              .Branch(OpCodes.Brtrue_S, out var firstHasValue)
              // if n2.HasValue goto returnZero
              .LoadLocal(secondHasValue)
              .Branch(OpCodes.Brfalse_S, out var returnZero)
              // return -1
              .Emit(OpCodes.Ldc_I4_M1)
              .Emit(OpCodes.Ret)
              // returnZero: return 0
              .MarkLabel(returnZero)
              .Emit(OpCodes.Ldc_I4_0)
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
              .Emit(OpCodes.Call, member.GetValueMethod)
              .Emit(OpCodes.Call, member.GetValueMethod);
        }
    }
}
