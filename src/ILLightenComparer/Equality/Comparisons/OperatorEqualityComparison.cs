using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Equality.Comparisons
{
    internal sealed class OperatorEqualityComparison : IComparisonEmitter
    {
        private readonly IVariable _variable;
        private readonly MethodInfo _equalityMethod;

        private OperatorEqualityComparison(IVariable variable, MethodInfo equalityMethod)
        {
            _variable = variable;
            _equalityMethod = equalityMethod;
        }

        public static OperatorEqualityComparison Create(IVariable variable)
        {
            var variableType = variable.VariableType.GetUnderlyingType();
            var equalityMethod = variableType.GetMethod("op_Equality", BindingFlags.Public | BindingFlags.Static);
            if (equalityMethod != null) {
                return new OperatorEqualityComparison(variable, equalityMethod);
            }

            return null;
        }

        public ILEmitter Emit(ILEmitter il, Label _) => il.Call(
            _equalityMethod,
            _variable.Load(Arg.X),
            _variable.Load(Arg.Y));

        public ILEmitter EmitCheckForResult(ILEmitter il, Label next) => il.EmitReturnIfFalsy(next);
    }
}
