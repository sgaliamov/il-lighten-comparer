using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Variables;

namespace ILLightenComparer.Emit.Emitters.Visitors.Comparisons
{
    internal interface IComparison
    {
        IVariable Variable { get; }

        ILEmitter LoadVariables(StackVisitor visitor, ILEmitter il, Label gotoNext);
        ILEmitter Accept(CompareEmitter visitor, ILEmitter il);
        ILEmitter Accept(CompareVisitor visitor, ILEmitter il);
    }
}
