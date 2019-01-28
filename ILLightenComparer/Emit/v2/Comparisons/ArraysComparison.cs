using System;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Variables;
using ILLightenComparer.Emit.v2.Visitors;

namespace ILLightenComparer.Emit.v2.Comparisons
{
    internal sealed class ArraysComparison : IComparison
    {
        private ArraysComparison(IVariable variable)
        {
            Variable = variable ?? throw new ArgumentNullException(nameof(variable));
        }

        public IVariable Variable { get; }

        public ILEmitter Accept(CompareVisitor visitor, ILEmitter il, Label gotoNext)
        {
            return visitor.Visit(this, il, gotoNext);
        }

        public static ArraysComparison Create(IVariable variable)
        {
            var variableType = variable.VariableType;
            if (variableType.IsArray && variableType.GetArrayRank() == 1)
            {
                return new ArraysComparison(variable);
            }

            return null;
        }
    }
}
