using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Comparer;
using ILLightenComparer.Extensions;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Shared.Comparisons
{
    internal sealed class NullableComparison : IComparisonEmitter
    {
        private readonly ComparisonResolver _resolver;
        private readonly IVariable _variable;

        private NullableComparison(ComparisonResolver resolver, IVariable variable)
        {
            _resolver = resolver;
            _variable = variable ?? throw new ArgumentNullException(nameof(variable));
        }

        public static NullableComparison Create(ComparisonResolver resolver, IVariable variable)
        {
            if (variable.VariableType.IsNullable()) {
                return new NullableComparison(resolver, variable);
            }

            return null;
        }

        public ILEmitter Emit(ILEmitter il, Label gotoNext)
        {
            var variableType = _variable.VariableType;

            _variable.Load(il, Arg.X).Store(variableType, out var nullableX);
            _variable.Load(il, Arg.Y).Store(variableType, out var nullableY);
            EmitCheckNullablesForValue(il, nullableX, nullableY, variableType, gotoNext);

            var nullableVariable = new NullableVariable(variableType, _variable.OwnerType, nullableX, nullableY);

            return _resolver
                .GetComparisonEmitter(nullableVariable)
                .Emit(il, gotoNext);
        }

        public ILEmitter Emit(ILEmitter il) => il
            .DefineLabel(out var exit)
            .Execute(this.Emit(exit))
            .EmitReturnIfTruthy(exit)
            .MarkLabel(exit)
            .Return(0);

        public ILEmitter EmitCheckForIntermediateResult(ILEmitter il, Label next) => il.EmitReturnIfTruthy(next);

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
