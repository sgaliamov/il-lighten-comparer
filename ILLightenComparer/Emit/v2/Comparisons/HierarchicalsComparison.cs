using System.Collections.Generic;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Variables;
using ILLightenComparer.Emit.v2.Visitors;

namespace ILLightenComparer.Emit.v2.Comparisons
{
    internal sealed class HierarchicalsComparison : IComparison
    {
        private HierarchicalsComparison(IVariable variable)
        {
            Variable = variable;
        }

        public IVariable Variable { get; }

        public ILEmitter Accept(CompareVisitor visitor, ILEmitter il, Label gotoNext)
        {
            return visitor.Visit(this, il);
        }

        public static HierarchicalsComparison Create(IVariable variable)
        {
            var type = variable.VariableType;
            if (type.IsPrimitive()
                || type.ImplementsGeneric(typeof(IEnumerable<>))
                || type.IsSealedComparable() // todo: IsComparable()?
                || type.IsNullable())
            {
                return null;
            }

            return new HierarchicalsComparison(variable);
        }
    }
}
