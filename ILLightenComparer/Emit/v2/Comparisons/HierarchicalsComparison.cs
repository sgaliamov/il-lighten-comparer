using System;
using System.Collections.Generic;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Visitors;

namespace ILLightenComparer.Emit.v2.Comparisons
{
    internal sealed class HierarchicalsComparison : IComparison
    {
        private HierarchicalsComparison(Type variableType)
        {
            VariableType = variableType;
        }

        public Type VariableType { get; }

        public ILEmitter Accept(CompareVisitor visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public static HierarchicalsComparison Create(Type variableType)
        {
            var underlyingType = variableType.GetUnderlyingType();
            if (underlyingType.IsPrimitive()
                || underlyingType.ImplementsGeneric(typeof(IEnumerable<>))
                || underlyingType.IsSealedComparable() // todo: IsComparable()?
                || underlyingType.IsNullable())
            {
                return null;
            }

            return new HierarchicalsComparison(variableType);
        }
    }
}
