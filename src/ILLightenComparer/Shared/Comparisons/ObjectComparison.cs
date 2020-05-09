using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Variables;
using Illuminator;

namespace ILLightenComparer.Shared.Comparisons
{
    internal sealed class ObjectComparison : IComparisonEmitter
    {
        public static ObjectComparison Create(IVariable variable) => (variable.VariableType == typeof(object))
            ? new ObjectComparison()
            : null;

        public ILEmitter Emit(ILEmitter il, Label _) => il.Return(0);

        public ILEmitter EmitCheckForResult(ILEmitter il, Label next) => il;
    }
}