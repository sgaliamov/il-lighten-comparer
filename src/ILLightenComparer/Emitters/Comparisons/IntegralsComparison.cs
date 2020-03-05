using System;
using System.Reflection.Emit;
using ILLightenComparer.Emitters.Variables;
using ILLightenComparer.Extensions;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Emitters.Comparisons
{
    internal sealed class IntegralsComparison : IComparison
    {
        private IntegralsComparison(IVariable variable) => Variable = variable;

        public IVariable Variable { get; }
        public bool PutsResultInStack => true;

        public ILEmitter Accept(ILEmitter il, Label gotoNext)
        {
            var variableType = Variable.VariableType;

            if (!variableType.GetUnderlyingType().IsIntegral()) {
                throw new InvalidOperationException($"Integral type is expected but: {variableType.DisplayName()}.");
            }

            return il.Sub(
                il => Variable.Load(il, Arg.X),
                il => Variable.Load(il, Arg.Y));
        }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

        public static IntegralsComparison Create(IVariable variable)
        {
            if (variable.VariableType.GetUnderlyingType().IsIntegral()) {
                return new IntegralsComparison(variable);
            }

            return null;
        }
    }
}
