using System;
using System.Reflection.Emit;
using ILLightenComparer.Emitters.Variables;
using ILLightenComparer.Emitters.Visitors;
using ILLightenComparer.Extensions;
using Illuminator;

namespace ILLightenComparer.Emitters.Comparisons
{
    internal sealed class MembersComparison : IComparison
    {
        private MembersComparison(IVariable variable) => Variable = variable;

        public IVariable Variable { get; }
        public bool PutsResultInStack => throw new NotSupportedException();

        public ILEmitter Accept(CompareVisitor visitor, ILEmitter il, Label gotoNext) => visitor.Visit(this, il);

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

        public static MembersComparison Create(IVariable variable)
        {
            if (variable.VariableType.IsHierarchical() && variable is ArgumentVariable) {
                return new MembersComparison(variable);
            }

            return null;
        }
    }
}
