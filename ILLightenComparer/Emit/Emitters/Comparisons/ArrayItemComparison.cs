using System;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Emitters.Visitors;

namespace ILLightenComparer.Emit.Emitters.Comparisons
{
    internal sealed class ArrayItemComparison : IComparisonAcceptor
    {
        public ArrayItemComparison(IVariable itemVariable, IComparisonAcceptor itemAcceptor)
        {
            Variable = itemVariable ?? throw new ArgumentNullException(nameof(itemVariable));
            ItemAcceptor = itemAcceptor ?? throw new ArgumentNullException(nameof(itemAcceptor));
        }

        public IComparisonAcceptor ItemAcceptor { get; }

        public ILEmitter Accept(CompareVisitor visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public IVariable Variable { get; }

        public ILEmitter LoadVariables(StackVisitor visitor, ILEmitter il, Label gotoNext)
        {
            return visitor.LoadVariables(this, il, gotoNext);
        }
    }
}
