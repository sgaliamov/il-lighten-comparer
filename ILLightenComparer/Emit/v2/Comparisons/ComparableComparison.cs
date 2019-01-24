using System;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Visitors;

namespace ILLightenComparer.Emit.v2.Comparisons
{
    internal sealed class ComparableComparison : IComparison
    {
        private ComparableComparison(Type variableType)
        {
            VariableType = variableType;
        }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public ILEmitter LoadVariables(StackVisitor visitor, ILEmitter il, Label gotoNext)
        {
            return visitor.LoadVariables(this, il, gotoNext);
        }

        public Type VariableType { get; }

        public ILEmitter Accept(CompareVisitor visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public static ComparableComparison Create(Type variableType)
        {
            var isComparable = variableType
                               .GetUnderlyingType()
                               .ImplementsGeneric(typeof(IComparable<>));
            if (isComparable)
            {
                return new ComparableComparison(variableType);
            }

            return null;
        }
    }
}
