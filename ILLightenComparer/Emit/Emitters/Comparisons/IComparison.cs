using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Emitters.Visitors;

namespace ILLightenComparer.Emit.Emitters.Comparisons
{
    internal interface IComparison
    {
        IVariable Variable { get; }
    }

    internal interface ICompareEmitterAcceptor : IComparison
    {
        ILEmitter Accept(CompareEmitter visitor, ILEmitter il);
    }

    internal interface IComparisonAcceptor : IComparison
    {
        ILEmitter Accept(CompareVisitor visitor, ILEmitter il);
        ILEmitter LoadVariables(StackVisitor visitor, ILEmitter il, Label gotoNext);
    }

    internal interface IMemberComparison : IComparisonAcceptor, ICompareEmitterAcceptor { }

    internal interface IStaticComparison : IMemberComparison { }
}
