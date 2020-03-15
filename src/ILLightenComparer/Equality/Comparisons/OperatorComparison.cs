using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Shared;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Equality.Comparisons
{
    internal sealed class OperatorComparison : IStepEmitter
    {
        private readonly IVariable _variable;
        private readonly MethodInfo _equalityMethod;

        private OperatorComparison(IVariable variable, MethodInfo equalityMethod)
        {
            _variable = variable;
            _equalityMethod = equalityMethod;
        }

        public static OperatorComparison Create(IVariable variable)
        {
            var variableType = variable.VariableType.GetUnderlyingType();
            var equalityMethod = variableType.GetMethod("op_Equality", BindingFlags.Public | BindingFlags.Static);
            if (equalityMethod != null) {
                return new OperatorComparison(variable, equalityMethod);
            }

            return null;
        }

        public bool PutsResultInStack { get; } = true;

        public ILEmitter Emit(ILEmitter il, Label _) => il.Call(
            _equalityMethod,
            _variable.Load(Arg.X),
            _variable.Load(Arg.Y));

        public ILEmitter Emit(ILEmitter il) => Emit(il, default).Return();
    }
}
