using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Emitters.Visitors;

namespace ILLightenComparer.Emit.Emitters.Comparisons
{
    internal sealed class ArrayItemComparison : IStackVisitorAcceptor, ICompareVisitorAcceptor
    {
        public ArrayItemComparison(ArrayItemVariable itemVariable, ICompareVisitorAcceptor itemAcceptor)
        {
            Variable = itemVariable;
            ItemAcceptor = itemAcceptor;
        }

        public ICompareVisitorAcceptor ItemAcceptor { get; }

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
