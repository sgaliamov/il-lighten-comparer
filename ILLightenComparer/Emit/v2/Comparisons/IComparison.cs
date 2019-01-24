using System;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Visitors;

namespace ILLightenComparer.Emit.v2.Comparisons
{
    internal interface IComparison
    {
        Type VariableType { get; }
        ILEmitter Accept(CompareVisitor visitor, ILEmitter il);
    }
}
