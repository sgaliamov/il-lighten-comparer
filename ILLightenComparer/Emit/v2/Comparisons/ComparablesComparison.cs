using System;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Visitors;

namespace ILLightenComparer.Emit.v2.Comparisons
{
    internal sealed class ComparablesComparison : IComparison
    {
        private ComparablesComparison(Type variableType)
        {
            VariableType = variableType;
        }

        public Type VariableType { get; }

        public ILEmitter Accept(CompareVisitor visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public static ComparablesComparison Create(Type variableType)
        {
            var isComparable = variableType
                               .GetUnderlyingType()
                               .ImplementsGeneric(typeof(IComparable<>));
            if (isComparable)
            {
                return new ComparablesComparison(variableType);
            }

            return null;
        }
    }
}
