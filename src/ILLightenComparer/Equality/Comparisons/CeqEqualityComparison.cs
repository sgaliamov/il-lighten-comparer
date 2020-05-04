using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Extensions;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Equality.Comparisons
{
    internal sealed class CeqEqualityComparison : IComparisonEmitter
    {
        private readonly IVariable _variable;

        private CeqEqualityComparison(IVariable variable) => _variable = variable;

        public static CeqEqualityComparison Create(IVariable variable)
        {
            if (variable.VariableType.GetUnderlyingType().IsBasicEquitable()) {
                return new CeqEqualityComparison(variable);
            }

            return null;
        }

        public ILEmitter Emit(ILEmitter il, Label _) => il.AreSame(_variable.Load(Arg.X), _variable.Load(Arg.Y));

        public ILEmitter EmitCheckForResult(ILEmitter il, Label next) => il.EmitReturnIfFalsy(next);
    }
}
