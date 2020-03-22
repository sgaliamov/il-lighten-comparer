using System.Reflection.Emit;
using ILLightenComparer.Extensions;
using ILLightenComparer.Shared;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Equality.Comparisons
{
    internal sealed class CeqComparison : IComparisonEmitter
    {
        private readonly IVariable _variable;

        private CeqComparison(IVariable variable) => _variable = variable;

        public static CeqComparison Create(IVariable variable)
        {
            if (variable.VariableType.GetUnderlyingType().IsBasicEquitable()) {
                return new CeqComparison(variable);
            }

            return null;
        }

        public bool PutsResultInStack { get; } = true;

        public ILEmitter Emit(ILEmitter il, Label _) => il
            .AreSame(_variable.Load(Arg.X), _variable.Load(Arg.Y));

        public ILEmitter Emit(ILEmitter il) => Emit(il, default).Return();
    }
}
