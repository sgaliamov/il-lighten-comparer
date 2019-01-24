using System;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Visitors;

namespace ILLightenComparer.Emit.v2.Comparisons
{
    internal sealed class StringComparison : IComparison
    {
        private StringComparison(Type variableType, int stringComparisonType)
        {
            VariableType = variableType;
            StringComparisonType = stringComparisonType;
        }

        public int StringComparisonType { get; }
        public Type VariableType { get; }

        public ILEmitter Accept(CompareVisitor visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public static StringComparison Create(Type variableType, int stringComparisonType)
        {
            if (variableType == typeof(string))
            {
                return new StringComparison(variableType, stringComparisonType);
            }

            return null;
        }
    }
}
