using System.Reflection.Emit;
using Illuminator;

namespace ILLightenComparer.Shared
{
    internal interface IComparison
    {
        // does the comparison puts a result into the stack.
        // comparisons with many items does not do it.
        bool PutsResultInStack { get; }

        /// <summary>
        /// Compare and leave a result in the stack.
        /// </summary>
        ILEmitter Emit(ILEmitter il, Label gotoNext);

        /// <summary>
        /// Compare and return.
        /// </summary>
        ILEmitter Emit(ILEmitter il);
    }
}
