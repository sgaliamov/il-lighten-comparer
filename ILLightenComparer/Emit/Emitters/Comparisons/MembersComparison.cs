using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Variables;
using ILLightenComparer.Emit.v2.Visitors;

namespace ILLightenComparer.Emit.v2.Comparisons
{
    internal sealed class MembersComparison : IComparison
    {
        private MembersComparison(IVariable variable)
        {
            Variable = variable;
        }

        public IVariable Variable { get; }

        public ILEmitter Accept(CompareVisitor visitor, ILEmitter il, Label gotoNext)
        {
            return visitor.Visit(this, il);
        }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public static MembersComparison Create(IVariable variable)
        {
            if (variable.VariableType.IsHierarchical() && variable is ArgumentVariable)
            {
                return new MembersComparison(variable);
            }

            return null;
        }
    }
}
