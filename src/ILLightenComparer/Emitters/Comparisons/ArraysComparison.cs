using System;
using System.Reflection.Emit;
using ILLightenComparer.Emitters.Variables;
using ILLightenComparer.Emitters.Visitors;
using Illuminator;

namespace ILLightenComparer.Emitters.Comparisons
{
    internal sealed class ArraysComparison : IComparison
    {
        private ArraysComparison(IVariable variable) => Variable = variable ?? throw new ArgumentNullException(nameof(variable));

        public IVariable Variable { get; }
        public bool PutsResultInStack => false;

        public ILEmitter Accept(CompareVisitor visitor, ILEmitter il, Label gotoNext) => visitor.Visit(this, il, gotoNext);

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

        public static ArraysComparison Create(IVariable variable)
        {
            var variableType = variable.VariableType;
            if (variableType.IsArray && variableType.GetArrayRank() == 1) {
                return new ArraysComparison(variable);
            }

            return null;
        }
    }
}
