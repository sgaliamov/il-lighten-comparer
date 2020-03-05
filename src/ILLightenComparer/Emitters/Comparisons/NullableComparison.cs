using System;
using System.Reflection.Emit;
using ILLightenComparer.Emitters.Variables;
using ILLightenComparer.Extensions;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Emitters.Comparisons
{
    internal sealed class NullableComparison : IComparison
    {
        private readonly ComparisonResolver _comparisons;
        private readonly IVariable _variable;

        private NullableComparison(ComparisonResolver comparisons, IVariable variable)
        {
            _comparisons = comparisons;
            _variable = variable ?? throw new ArgumentNullException(nameof(variable));
        }
        
        public static NullableComparison Create(ComparisonResolver comparisons, IVariable variable)
        {
            if (variable.VariableType.IsNullable()) {
                return new NullableComparison(comparisons, variable);
            }

            return null;
        }

        public bool PutsResultInStack => true;

        public ILEmitter Compare(ILEmitter il, Label gotoNext)
        {
            var variableType = _variable.VariableType;

            _variable.Load(il, Arg.X).Store(variableType, out var nullableX);
            _variable.Load(il, Arg.Y).Store(variableType, out var nullableY);
            il.EmitCheckNullablesForValue(nullableX, nullableY, variableType, gotoNext);

            var nullableVariable = new NullableVariable(variableType, _variable.OwnerType, nullableX, nullableY);

            return _comparisons
                   .GetComparison(nullableVariable)
                   .Compare(il, gotoNext);
        }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
    }
}
