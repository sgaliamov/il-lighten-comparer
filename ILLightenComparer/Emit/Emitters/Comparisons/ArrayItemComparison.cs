using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Emitters.Visitors;

namespace ILLightenComparer.Emit.Emitters.Comparisons
{
    internal sealed class ArrayItemComparison : IComparisonAcceptor
    {
        public ArrayItemComparison(ArrayItemVariable itemVariable, IComparisonAcceptor itemAcceptor)
        {
            Variable = itemVariable;
            ItemAcceptor = itemAcceptor;
        }

        public IComparisonAcceptor ItemAcceptor { get; }

        public ILEmitter Accept(CompareVisitor visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        /// <summary>
        ///     ArrayItemVariable.
        /// </summary>
        public IVariable Variable { get; }

        public ILEmitter LoadVariables(StackVisitor visitor, ILEmitter il, Label gotoNext)
        {
            return visitor.LoadVariables(this, il, gotoNext);
        }
    }
}
