using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Variables;
using Illuminator;
using static Illuminator.Functional;

namespace ILLightenComparer.Shared.Comparisons
{
    internal sealed class ObjectComparison : IComparisonEmitter
    {
        private readonly EmitReferenceComparisonDelegate _emitReferenceComparison;
        private readonly IVariable _variable;

        public ObjectComparison(EmitReferenceComparisonDelegate emitReferenceComparison, IVariable variable)
        {
            _emitReferenceComparison = emitReferenceComparison;
            _variable = variable;
        }

        public static ObjectComparison Create(EmitReferenceComparisonDelegate emitReferenceComparison, IVariable variable) => (variable.VariableType == typeof(object))
            ? new ObjectComparison(emitReferenceComparison, variable)
            : null;

        public ILEmitter Emit(ILEmitter il, Label next) => il;

        public ILEmitter EmitCheckForResult(ILEmitter il, Label next) => _emitReferenceComparison(il, _variable.Load(Arg.X), _variable.Load(Arg.Y), GoTo(next)).GoTo(next);
    }
}