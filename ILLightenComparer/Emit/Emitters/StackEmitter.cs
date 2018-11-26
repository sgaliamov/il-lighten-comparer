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
            il.LoadProperty(member, 1)
              .TempLocal(member.GetterMethod.ReturnType, out var local)
              .Store(local)
              .LoadAddress(local)
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
            var propertyType = member.GetterMethod.ReturnType;

            il.LoadProperty(member, 2) // var n2 = &arg2
              .DeclareLocal(propertyType, out var n2)
              .Store(n2)
              .LoadAddress(n2)
              // var secondHasValue = n2->HasValue
              .Call(propertyType, member.HasValueMethod)
              .TempLocal(typeof(bool), out var secondHasValue)
              .Store(secondHasValue)
              // var n1 = &arg1
              .LoadProperty(member, 1)
              .DeclareLocal(propertyType, out var n1)
              .Store(n1)
              .LoadAddress(n1)
              // if n1->HasValue goto firstHasValue
              .Call(propertyType, member.HasValueMethod)
              .Branch(OpCodes.Brtrue_S, out var firstHasValue)
              // if n2->HasValue goto returnZero
              .LoadLocal(secondHasValue)
              .Branch(OpCodes.Brfalse_S, out var returnZero)
              // else return -1
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
              .LoadAddress(n1)
              .Call(propertyType, member.GetValueMethod)
              .LoadAddress(n2)
              .Call(propertyType, member.GetValueMethod);
        }
    }
}
