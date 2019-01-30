using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Emitters.Visitors;
using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit.Emitters.Comparisons
{
    internal sealed class StringsComparison : IComparison
    {
        private StringsComparison(IVariable variable)
        {
            Variable = variable;
        }

        public IVariable Variable { get; }
        public bool ResultInStack => true;

        public ILEmitter Accept(CompareVisitor visitor, ILEmitter il, Label gotoNext)
        {
            return visitor.Visit(this, il);
        }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public static StringsComparison Create(IVariable variable)
        {
            if (variable.VariableType == typeof(string))
            {
                return new StringsComparison(variable);
            }

            return null;
        }
    }
}
