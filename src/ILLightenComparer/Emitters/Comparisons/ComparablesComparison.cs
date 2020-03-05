using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emitters.Variables;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Emitters.Comparisons
{
    internal sealed class ComparablesComparison : IComparison
    {
        private readonly MethodInfo _compareToMethod;

        private ComparablesComparison(IVariable variable)
        {
            Variable = variable;

            _compareToMethod = variable.VariableType.GetUnderlyingCompareToMethod()
                              ?? throw new ArgumentException(
                                  $"{variable.VariableType.DisplayName()} does not have {MethodName.CompareTo} method.");
        }

        public IVariable Variable { get; }
        public bool PutsResultInStack => true;

        public ILEmitter Accept(ILEmitter il, Label gotoNext)
        {
            var variableType = Variable.VariableType;

            if (variableType.IsValueType) {
                Variable.LoadAddress(il, Arg.X);
                Variable.Load(il, Arg.Y);
            } else {
                Variable.Load(il, Arg.X).Store(variableType, out var x);
                Variable.Load(il, Arg.Y)
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

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

        public static ComparablesComparison Create(IVariable variable)
        {
            // todo: if object implements IComparable, then it should be used anyway?
            if (variable.VariableType.GetUnderlyingType().IsSealedComparable()) {
                return new ComparablesComparison(variable);
            }

            return null;
        }
    }
}
