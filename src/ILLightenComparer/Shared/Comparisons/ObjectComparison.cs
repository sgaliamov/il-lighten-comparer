using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Variables;
using Illuminator;

namespace ILLightenComparer.Shared.Comparisons
{
    internal sealed class ObjectComparison : IComparisonEmitter
    {
        private readonly IVariable _variable;

        public ObjectComparison(IVariable variable) => _variable = variable;

        public static ObjectComparison Create(IVariable variable) => (variable.VariableType == typeof(object))
            ? new ObjectComparison(variable)
            : null;

        public ILEmitter Emit(ILEmitter il, Label next) => il
            .Execute(_variable.Load(Arg.Y))
            .IfTrue_S(_variable.Load(Arg.X), out var xIsNotNull)
            .IfFalse_S(next)
            .Return(-1)
            .MarkLabel(xIsNotNull)
            .IfTrue_S(next)
            .Return(1);

        public ILEmitter EmitCheckForResult(ILEmitter il, Label next) => il;
    }
}