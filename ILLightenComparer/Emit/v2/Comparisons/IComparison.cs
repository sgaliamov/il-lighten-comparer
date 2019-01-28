using System.Reflection.Emit;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Variables;
using ILLightenComparer.Emit.v2.Visitors;

namespace ILLightenComparer.Emit.v2.Comparisons
{
    internal interface IComparison
    {
        IVariable Variable { get; }
        ILEmitter Accept(CompareVisitor visitor, ILEmitter il, Label gotoNext);
        ILEmitter Accept(CompareEmitter visitor, ILEmitter il);
    }
}
