using System.Reflection.Emit;
using Illuminator;

namespace ILLightenComparer.Emitters.Comparisons
{
    internal interface IComparison
    {
        bool PutsResultInStack { get; }

        ILEmitter Compare(ILEmitter il, Label gotoNext);
        ILEmitter Accept(CompareEmitter visitor, ILEmitter il);
    }
}
