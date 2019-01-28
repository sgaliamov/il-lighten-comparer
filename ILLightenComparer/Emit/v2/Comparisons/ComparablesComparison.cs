using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Variables;
using ILLightenComparer.Emit.v2.Visitors;

namespace ILLightenComparer.Emit.v2.Comparisons
{
    internal sealed class ComparablesComparison : IComparison
    {
        private ComparablesComparison(IVariable variable)
        {
            Variable = variable;

            CompareToMethod = variable.VariableType.GetUnderlyingCompareToMethod()
                              ?? throw new ArgumentException(
                                  $"{variable.VariableType.DisplayName()} does not have {MethodName.CompareTo} method.");
        }

        public MethodInfo CompareToMethod { get; }
        public IVariable Variable { get; }

        public ILEmitter Accept(CompareVisitor visitor, ILEmitter il, Label gotoNext)
        {
            return visitor.Visit(this, il, gotoNext);
        }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public static ComparablesComparison Create(IVariable variable)
        {
            // todo: if object implements IComparable, then it should be used anyway?
            if (variable.VariableType.IsSealedComparable())
            {
                return new ComparablesComparison(variable);
            }

            return null;
        }
    }
}
