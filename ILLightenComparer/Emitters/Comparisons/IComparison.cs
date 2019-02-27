using System.Reflection.Emit;
using ILLightenComparer.Emitters.Variables;
using ILLightenComparer.Emitters.Visitors;
using ILLightenComparer.Shared;

namespace ILLightenComparer.Emitters.Comparisons
{
    internal interface IComparison
    {
        IVariable Variable { get; }
        ILEmitter Accept(CompareVisitor visitor, ILEmitter il, Label gotoNext);
        ILEmitter Accept(CompareEmitter visitor, ILEmitter il);
        bool ResultInStack { get; }
    }
}
