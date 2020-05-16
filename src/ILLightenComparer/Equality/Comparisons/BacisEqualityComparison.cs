using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Extensions;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;
using static Illuminator.Functional;

namespace ILLightenComparer.Equality.Comparisons
{
    internal sealed class BacisEqualityComparison : IComparisonEmitter
    {
        private readonly IVariable _variable;
        private readonly MethodInfo _equalityMethod;

        private BacisEqualityComparison(IVariable variable)
        {
            _variable = variable;
            _equalityMethod = _variable.VariableType.GetMethod(nameof(Equals), BindingFlags.Public | BindingFlags.Instance);
        }

        public static BacisEqualityComparison Create(IVariable variable)
        {
            var variableType = variable.VariableType.GetUnderlyingType();
            if (variableType.IsBasic()) {
                return new BacisEqualityComparison(variable);
            }

            return null;
        }

        public ILEmitter Emit(ILEmitter il, Label _) => il.Call(
            _equalityMethod,
            ExecuteIf(_variable.VariableType.IsValueType, _variable.LoadAddress(Arg.X)),
            ExecuteIf(!_variable.VariableType.IsValueType, _variable.Load(Arg.X)),
            _variable.Load(Arg.Y));

        public ILEmitter EmitCheckForResult(ILEmitter il, Label next) => il.EmitReturnIfFalsy(next);
    }
}
