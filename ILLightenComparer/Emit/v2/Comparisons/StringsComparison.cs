using System;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Visitors;

namespace ILLightenComparer.Emit.v2.Comparisons
{
    internal sealed class StringsComparison : IComparison
    {
        private StringsComparison(Type variableType, StringComparison stringComparisonType)
        {
            VariableType = variableType;
            StringComparisonType = (int)stringComparisonType;
        }

        public int StringComparisonType { get; }
        public Type VariableType { get; }

        public ILEmitter Accept(CompareVisitor visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public static StringsComparison Create(Type variableType, StringComparison stringComparisonType)
        {
            if (variableType == typeof(string))
            {
                return new StringsComparison(variableType, stringComparisonType);
            }

            return null;
        }
    }
}
