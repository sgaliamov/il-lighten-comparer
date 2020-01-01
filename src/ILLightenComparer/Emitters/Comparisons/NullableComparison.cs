using System;
using System.Reflection.Emit;
using ILLightenComparer.Emitters.Variables;
using ILLightenComparer.Emitters.Visitors;
using ILLightenComparer.Extensions;
using ILLightenComparer.Shared;

namespace ILLightenComparer.Emitters.Comparisons
{
    internal sealed class NullableComparison : IComparison
    {
        private NullableComparison(IVariable variable) => Variable = variable ?? throw new ArgumentNullException(nameof(variable));

        public IVariable Variable { get; }
        public bool PutsResultInStack => true;

        public ILEmitter Accept(CompareVisitor visitor, ILEmitter il, Label gotoNext) => visitor.Visit(this, il, gotoNext);

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

        public static NullableComparison Create(IVariable variable) {
            if (variable.VariableType.IsNullable()) {
                return new NullableComparison(variable);
            }

            return null;
        }
    }
}
