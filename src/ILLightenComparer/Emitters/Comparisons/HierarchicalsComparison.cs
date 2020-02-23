using System.Reflection.Emit;
using ILLightenComparer.Emitters.Variables;
using ILLightenComparer.Emitters.Visitors;
using ILLightenComparer.Extensions;
using Illuminator;

namespace ILLightenComparer.Emitters.Comparisons
{
    internal sealed class HierarchicalsComparison : IComparison
    {
        private HierarchicalsComparison(IVariable variable) => Variable = variable;

        public IVariable Variable { get; }
        public bool PutsResultInStack => true;

        public ILEmitter Accept(CompareVisitor visitor, ILEmitter il, Label gotoNext) => visitor.Visit(this, il);

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

        public static HierarchicalsComparison Create(IVariable variable)
        {
            if (variable.VariableType.IsHierarchical() && !(variable is ArgumentVariable)) {
                return new HierarchicalsComparison(variable);
            }

            return null;
        }
    }
}
