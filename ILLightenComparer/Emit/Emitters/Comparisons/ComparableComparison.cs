using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Emitters.Visitors;
using ILLightenComparer.Emit.Emitters.Visitors.Comparisons;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.Emitters.Comparisons
{
    internal sealed class ComparableComparison : IComparableComparison
    {
        private ComparableComparison(IVariable variable)
        {
            Variable = variable;
        }

        public IVariable Variable { get; }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public ILEmitter LoadVariables(StackVisitor visitor, ILEmitter il, Label gotoNext)
        {
            return visitor.Visit(this, il, gotoNext);
        }

        public ILEmitter Accept(CompareVisitor visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public static ComparableComparison Create(MemberInfo memberInfo)
        {
            var variable = VariableFactory.Create(memberInfo);

            var underlyingType = variable.VariableType.GetUnderlyingType();
            var isComparable = underlyingType.ImplementsGeneric(typeof(IComparable<>));
            if (isComparable)
            {
                return new ComparableComparison(variable);
            }

            return null;
        }
    }
}
