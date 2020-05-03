using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Extensions;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Comparer.Comparisons
{
    internal sealed class ComparablesComparison : IComparisonEmitter
    {
        private readonly MethodInfo _compareToMethod;
        private readonly IVariable _variable;

        private ComparablesComparison(IVariable variable)
        {
            _variable = variable;
            _compareToMethod = _variable.VariableType.GetUnderlyingCompareToMethod();
        }

        public static ComparablesComparison Create(IVariable variable)
        {
            // todo: 1. if object implements IComparable, then it should be used anyway, create setting to enable it
            if (variable.VariableType.GetUnderlyingType().IsSealedComparable()) {
                return new ComparablesComparison(variable);
            }

            return null;
        }

        public ILEmitter Emit(ILEmitter il, Label gotoNext)
        {
            var variableType = _variable.VariableType;

            if (variableType.IsValueType) {
                _variable.LoadAddress(il, Arg.X);
                _variable.Load(il, Arg.Y);
            } else {
                _variable.Load(il, Arg.X).Store(variableType, out var x);
                _variable.Load(il, Arg.Y)
                         .Store(variableType, out var y)
                         .LoadLocal(x)
                         .IfTrue_S(out var call)
                         .LoadLocal(y)
                         .IfFalse_S(gotoNext)
                         .Return(-1)
                         .MarkLabel(call)
                         .LoadLocal(x)
                         .LoadLocal(y);
            }

            return il.Call(_compareToMethod);
        }

        public ILEmitter EmitCheckForResult(ILEmitter il, Label next) => il.EmitReturnIfTruthy(next);
    }
}
