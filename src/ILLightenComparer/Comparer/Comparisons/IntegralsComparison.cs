using System.Diagnostics;
using System.Reflection.Emit;
using ILLightenComparer.Extensions;
using ILLightenComparer.Shared;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Comparer.Comparisons
{
    internal sealed class IntegralsComparison : IComparisonEmitter
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

        public bool PutsResultInStack { get; } = true;

        public ILEmitter Emit(ILEmitter il, Label _)
        {
            Debug.Assert(
                _variable.VariableType.GetUnderlyingType().IsIntegral(),
                $"Integral type is expected but: {_variable.VariableType.DisplayName()}.");

            return il.Sub(
                il => _variable.Load(il, Arg.X),
                il => _variable.Load(il, Arg.Y));
        }

        public ILEmitter Emit(ILEmitter il) => Emit(il, default).Return();
    }
}
