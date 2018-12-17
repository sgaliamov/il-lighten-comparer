using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Emitters.Visitors;

namespace ILLightenComparer.Emit.Emitters.Comparisons
{
    internal interface IComparison
    {
        IVariable Variable { get; }
    }

    internal interface IStaticComparison : IComparisonAcceptor { }

    internal interface IComparisonAcceptor :
        ICompareVisitorAcceptor, 
        ICompareEmitterAcceptor, 
        IStackVisitorAcceptor { }

    internal interface ICompareVisitorAcceptor : IComparison
    {
        ILEmitter Accept(CompareVisitor visitor, ILEmitter il);
    }

    internal interface ICompareEmitterAcceptor : IComparison
    {
        ILEmitter Accept(CompareEmitter visitor, ILEmitter il);
    }

    internal interface IStackVisitorAcceptor : IComparison
    {
        ILEmitter LoadVariables(StackVisitor visitor, ILEmitter il, Label gotoNext);
    }
}
