using System.Reflection.Emit;
using Illuminator;

namespace ILLightenComparer.Emitters.Comparisons
{
    internal interface IComparison
    {
        // does the comparison puts a result into the stack.
        // comparisons with many items does not do it.
        bool PutsResultInStack { get; }

        /// <summary>
        /// Compare and leave a result in the stack.
        /// </summary>
        ILEmitter Compare(ILEmitter il, Label gotoNext);

        /// <summary>
        /// Compare and return.
        /// </summary>
        ILEmitter Compare(ILEmitter il);
    }
}
