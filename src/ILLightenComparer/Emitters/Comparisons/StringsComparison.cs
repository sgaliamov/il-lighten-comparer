using System.Reflection.Emit;
using ILLightenComparer.Emitters.Variables;
using ILLightenComparer.Emitters.Visitors;
using Illuminator;

namespace ILLightenComparer.Emitters.Comparisons
{
    internal sealed class StringsComparison : IComparison
    {
        private StringsComparison(IVariable variable) => Variable = variable;

        public IVariable Variable { get; }
        public bool PutsResultInStack => true;

        public ILEmitter Accept(CompareVisitor visitor, ILEmitter il, Label gotoNext) => visitor.Visit(this, il);

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

        public static StringsComparison Create(IVariable variable)
        {
            if (variable.VariableType == typeof(string)) {
                return new StringsComparison(variable);
            }

            return null;
        }
    }
}
