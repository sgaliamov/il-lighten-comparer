using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Extensions;
using ILLightenComparer.Shared;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Comparer.Comparisons
{
    internal sealed class NullableComparison : IComparisonEmitter
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

        public bool PutsResultInStack { get; } = true;

        public ILEmitter Emit(ILEmitter il, Label gotoNext)
        {
            var variableType = _variable.VariableType;

            _variable.Load(il, Arg.X).Store(variableType, out var nullableX);
            _variable.Load(il, Arg.Y).Store(variableType, out var nullableY);
            EmitCheckNullablesForValue(il, nullableX, nullableY, variableType, gotoNext);

            var nullableVariable = new NullableVariable(variableType, _variable.OwnerType, nullableX, nullableY);

            return _comparisons
                .GetComparison(nullableVariable)
                .Emit(il, gotoNext);
        }

        public ILEmitter Emit(ILEmitter il)
        {
            il.DefineLabel(out var exit);

            return Emit(il, exit)
                  .EmitReturnIfTruthy(exit)
                  .MarkLabel(exit)
                  .Return(0);
        }

        private static ILEmitter EmitCheckNullablesForValue(
            ILEmitter il,
            LocalVariableInfo nullableX,
            LocalVariableInfo nullableY,
            Type nullableType,
            Label ifBothNull)
        {
            var hasValueMethod = nullableType.GetPropertyGetter("HasValue");

            return il.LoadAddress(nullableY)
                     .Call(hasValueMethod)
                     .Store(typeof(bool), out var secondHasValue)
                     .LoadAddress(nullableX)
                     .Call(hasValueMethod)
                     .IfTrue_S(out var ifFirstHasValue)
                     .LoadLocal(secondHasValue)
                     .IfFalse_S(ifBothNull)
                     .Return(-1)
                     .MarkLabel(ifFirstHasValue)
                     .LoadLocal(secondHasValue)
                     .IfTrue_S(out var next)
                     .Return(1)
                     .MarkLabel(next);
        }
    }
}
