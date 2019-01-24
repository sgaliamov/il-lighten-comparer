using System;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Visitors;

namespace ILLightenComparer.Emit.v2.Comparisons
{
    internal sealed class IntegralsComparison : IComparison
    {
        private IntegralsComparison(Type variableType)
        {
            VariableType = variableType;
        }

        public Type VariableType { get; }

        public ILEmitter Accept(CompareVisitor visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public static IntegralsComparison Create(Type variableType)
        {
            if (variableType.GetUnderlyingType().IsIntegral())
            {
                return new IntegralsComparison(variableType);
            }

            return null;
        }
    }
}
