using System.Reflection.Emit;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Variables;
using ILLightenComparer.Emit.v2.Visitors;

namespace ILLightenComparer.Emit.v2.Comparisons
{
    internal interface IVariableComparison
    {
        IVariable Variable { get; }
    }

    internal interface ICompareEmitterAcceptor : IVariableComparison
    {
        ILEmitter Accept(CompareEmitter visitor, ILEmitter il);
    }

    internal interface IVisitorsAcceptor : IVariableComparison
    {
        ILEmitter Accept(CompareVisitor visitor, ILEmitter il);
        ILEmitter LoadVariables(StackVisitor visitor, ILEmitter il, Label gotoNext);
    }

    internal interface IComparison : IVisitorsAcceptor, ICompareEmitterAcceptor { }

    internal interface IStaticComparison : IComparison { }
}
