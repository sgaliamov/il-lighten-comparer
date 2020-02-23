using System.Reflection.Emit;
using ILLightenComparer.Emitters.Variables;
using ILLightenComparer.Emitters.Visitors;
using Illuminator;

namespace ILLightenComparer.Emitters.Comparisons
{
    internal sealed class CustomComparison : IComparison
    {
        public CustomComparison(IVariable variable) => Variable = variable;

        public bool PutsResultInStack => true;
        public IVariable Variable { get; }

        public ILEmitter Accept(CompareVisitor visitor, ILEmitter il, Label gotoNext) => visitor.Visit(this, il);

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
    }
}
