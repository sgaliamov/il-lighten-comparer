using System;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Emitters.Visitors;

namespace ILLightenComparer.Emit.Emitters.Comparisons
{
    internal sealed class VariableComparison : IMemberComparison
    {
        public VariableComparison(IVariable itemVariable, IComparisonAcceptor itemAcceptor)
        {
            Variable = itemVariable ?? throw new ArgumentNullException(nameof(itemVariable));
            Acceptor = itemAcceptor ?? throw new ArgumentNullException(nameof(itemAcceptor));
        }

        public IComparisonAcceptor Acceptor { get; }
        public IVariable Variable { get; }

        public ILEmitter Accept(CompareVisitor visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public ILEmitter LoadVariables(StackVisitor visitor, ILEmitter il, Label gotoNext)
        {
            return visitor.LoadVariables(this, il, gotoNext);
        }
    }
}
