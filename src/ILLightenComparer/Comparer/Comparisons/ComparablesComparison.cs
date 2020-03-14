using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using ILLightenComparer.Shared;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Comparer.Comparisons
{
    internal sealed class ComparablesComparison : IStepEmitter
    {
        private readonly MethodInfo _compareToMethod;
        private readonly IVariable _variable;

        private ComparablesComparison(IVariable variable)
        {
            _variable = variable;

            _compareToMethod = variable.VariableType.GetUnderlyingCompareToMethod()
                              ?? throw new ArgumentException(
                                  $"{variable.VariableType.DisplayName()} does not have {MethodName.CompareTo} method.");
        }

        public static ComparablesComparison Create(IVariable variable)
        {
            // todo: 1. if object implements IComparable, then it should be used anyway?
            if (variable.VariableType.GetUnderlyingType().IsSealedComparable()) {
                return new ComparablesComparison(variable);
            }

            return null;
        }

        public bool PutsResultInStack => true;

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
                        .Branch(OpCodes.Brtrue_S, out var call)
                        .LoadLocal(y)
                        .Branch(OpCodes.Brfalse_S, gotoNext)
                        .Return(-1)
                        .MarkLabel(call)
                        .LoadLocal(x)
                        .LoadLocal(y);
            }

            return il.Call(_compareToMethod);
        }

        public ILEmitter Emit(ILEmitter il)
        {
            il.DefineLabel(out var exit);

            return Emit(il, exit)
                .EmitReturnNotZero(exit)
                .MarkLabel(exit)
                .Return(0);
        }
    }
}
