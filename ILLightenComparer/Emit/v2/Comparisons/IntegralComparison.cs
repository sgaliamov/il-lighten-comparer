using System;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Visitors;

namespace ILLightenComparer.Emit.v2.Comparisons
{
    internal sealed class IntegralComparison : IComparison
    {
        private IntegralComparison(Type variableType)
        {
            VariableType = variableType;
        }

        public Type VariableType { get; }

        public ILEmitter Accept(CompareVisitor visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public static IntegralComparison Create(Type variableType)
        {
            if (variableType.GetUnderlyingType().IsIntegral())
            {
                return new IntegralComparison(variableType);
            }

            return null;
        }
    }
}
