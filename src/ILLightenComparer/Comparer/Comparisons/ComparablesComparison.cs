using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Extensions;
using ILLightenComparer.Variables;
using Illuminator;
using static ILLightenComparer.Extensions.Functions;
using static Illuminator.Functions;

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
            if (variable.VariableType.GetUnderlyingType().IsComparable()) {
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
                _variable.Load(il, Arg.X).Stloc(variableType, out var x);
                _variable.Load(il, Arg.Y).Stloc(variableType, out var y)
                         .Ldloc(x)
                         .Brtrue_S(out var call)
                         .Ldloc(y)
                         .Brfalse_S(gotoNext)
                         .Ret(-1)
                         .MarkLabel(call)
                         .Ldloc(x)
                         .Ldloc(y);
            }

            return il.CallMethod(_compareToMethod, new[] { _variable.VariableType });
        }

        public ILEmitter EmitCheckForResult(ILEmitter il, Label next) => il.EmitReturnIfTruthy(next);
    }
}
