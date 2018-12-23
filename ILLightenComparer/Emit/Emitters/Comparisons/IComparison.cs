using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Emitters.Visitors;

namespace ILLightenComparer.Emit.Emitters.Comparisons
{
    internal interface IVariableComparison
    {
        IVariable Variable { get; }
    }

    internal interface ICompareEmitterAcceptor : IVariableComparison
    {
        ILEmitter Accept(CompareEmitter visitor, ILEmitter il);
    }

    internal interface IComparisonAcceptor : IVariableComparison
    {
        ILEmitter Accept(CompareVisitor visitor, ILEmitter il);
        ILEmitter LoadVariables(StackVisitor visitor, ILEmitter il, Label gotoNext);
    }

    internal interface IComparison : IComparisonAcceptor, ICompareEmitterAcceptor { }

    internal interface IStaticComparison : IComparison { }
}
