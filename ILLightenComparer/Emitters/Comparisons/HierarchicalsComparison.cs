using System.Reflection.Emit;
using ILLightenComparer.Emitters.Variables;
using ILLightenComparer.Emitters.Visitors;
using ILLightenComparer.Extensions;
using ILLightenComparer.Shared;

namespace ILLightenComparer.Emitters.Comparisons
{
    internal sealed class HierarchicalsComparison : IComparison
    {
        private HierarchicalsComparison(IVariable variable)
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

        public static HierarchicalsComparison Create(IVariable variable)
        {
            if (variable.VariableType.IsHierarchical() && !(variable is ArgumentVariable))
            {
                return new HierarchicalsComparison(variable);
            }

            return null;
        }
    }
}
