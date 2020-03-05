using System.Reflection.Emit;
using ILLightenComparer.Emitters.Variables;
using Illuminator;

namespace ILLightenComparer.Emitters.Comparisons
{
    internal interface IComparison
    {
        bool PutsResultInStack { get; }
        IVariable Variable { get; }

        ILEmitter Accept(ILEmitter il, Label gotoNext);
        ILEmitter Accept(CompareEmitter visitor, ILEmitter il);
    }
}
