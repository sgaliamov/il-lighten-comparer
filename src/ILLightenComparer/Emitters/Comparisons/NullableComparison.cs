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

        private NullableComparison(ComparisonResolver comparisons, IVariable variable)
        {
            _comparisons = comparisons;
            Variable = variable ?? throw new ArgumentNullException(nameof(variable));
        }

        public IVariable Variable { get; }
        public bool PutsResultInStack => true;

        public ILEmitter Accept(ILEmitter il, Label gotoNext)
        {
            var variableType = Variable.VariableType;

            Variable.Load(il, Arg.X).Store(variableType, out var nullableX);
            Variable.Load(il, Arg.Y).Store(variableType, out var nullableY);
            il.EmitCheckNullablesForValue(nullableX, nullableY, variableType, gotoNext);

            var nullableVariable = new NullableVariable(variableType, Variable.OwnerType, nullableX, nullableY);

            return _comparisons
                   .GetComparison(nullableVariable)
                   .Accept(il, gotoNext);
        }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

        public static NullableComparison Create(ComparisonResolver comparisons, IVariable variable)
        {
            if (variable.VariableType.IsNullable()) {
                return new NullableComparison(comparisons, variable);
            }

            return null;
        }
    }
}
