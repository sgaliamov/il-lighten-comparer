using System;
using System.Reflection.Emit;
using ILLightenComparer.Extensions;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Comparer.Comparisons
{
    internal sealed class IntegralsComparison : IComparison
    {
        private readonly IVariable _variable;

        private IntegralsComparison(IVariable variable) => _variable = variable;

        public static IntegralsComparison Create(IVariable variable)
        {
            if (variable.VariableType.GetUnderlyingType().IsIntegral()) {
                return new IntegralsComparison(variable);
            }

            return null;
        }

        public bool PutsResultInStack => true;

        public ILEmitter Emit(ILEmitter il, Label _)
        {
            var variableType = _variable.VariableType;

            if (!variableType.GetUnderlyingType().IsIntegral()) {
                throw new InvalidOperationException($"Integral type is expected but: {variableType.DisplayName()}.");
            }

            return il.Sub(
                il => _variable.Load(il, Arg.X),
                il => _variable.Load(il, Arg.Y));
        }

        public ILEmitter Emit(ILEmitter il) => Emit(il, default).Return();
    }
}
