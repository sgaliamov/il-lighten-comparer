using System;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Variables;
using ILLightenComparer.Emit.v2.Visitors;

namespace ILLightenComparer.Emit.v2.Comparisons
{
    internal sealed class StringsComparison : IComparison
    {
        private StringsComparison(IVariable variable, StringComparison stringComparisonType)
        {
            Variable = variable;
            StringComparisonType = (int)stringComparisonType;
        }

        public int StringComparisonType { get; }
        public IVariable Variable { get; }

        public ILEmitter Accept(CompareVisitor visitor, ILEmitter il, Label gotoNext)
        {
            return visitor.Visit(this, il);
        }

        public static StringsComparison Create(IVariable variable, StringComparison stringComparisonType)
        {
            if (variable.VariableType == typeof(string))
            {
                return new StringsComparison(variable, stringComparisonType);
            }

            return null;
        }
    }
}
