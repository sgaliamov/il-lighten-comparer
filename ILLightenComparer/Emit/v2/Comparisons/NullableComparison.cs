using System;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Variables;
using ILLightenComparer.Emit.v2.Visitors;

namespace ILLightenComparer.Emit.v2.Comparisons
{
    internal sealed class NullableComparison : IComparison
    {
        private NullableComparison(IVariable variable)
        {
            Variable = variable ?? throw new ArgumentNullException(nameof(variable));
        }

        public IVariable Variable { get; }

        public ILEmitter Accept(CompareVisitor visitor, ILEmitter il, Label gotoNext)
        {
            return visitor.Visit(this, il, gotoNext);
        }

        public static NullableComparison Create(IVariable variable)
        {
            if (variable.VariableType.IsNullable())
            {
                return new NullableComparison(variable);
            }

            return null;
        }
    }
}
