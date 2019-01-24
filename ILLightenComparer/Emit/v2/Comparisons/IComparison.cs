using System;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Visitors;

namespace ILLightenComparer.Emit.v2.Comparisons
{
    internal interface ICompareEmitterAcceptor
    {
        ILEmitter Accept(CompareEmitter visitor, ILEmitter il);
    }

    internal interface IVisitorsAcceptor
    {
        Type VariableType { get; }
        ILEmitter Accept(CompareVisitor visitor, ILEmitter il);
        ILEmitter LoadVariables(StackVisitor visitor, ILEmitter il, Label gotoNext);
    }

    internal interface IComparison : IVisitorsAcceptor, ICompareEmitterAcceptor { }

    internal interface IStaticComparison : IComparison { }
}
