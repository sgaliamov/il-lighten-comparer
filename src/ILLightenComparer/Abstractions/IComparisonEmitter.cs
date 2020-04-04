using System.Reflection.Emit;
using Illuminator;

namespace ILLightenComparer.Abstractions
{
    internal interface IComparisonEmitter
    {
        /// <summary>
        ///     Does the comparison puts a result into the stack.
        ///     Comparisons with many items does not do it.
        /// </summary>
        bool PutsResultInStack { get; }

        /// <summary>
        ///     Compare and leave a result in the stack.
        /// </summary>
        ILEmitter Emit(ILEmitter il, Label gotoNext);

        /// <summary>
        ///     Compare and return.
        /// </summary>
        ILEmitter Emit(ILEmitter il);
    }
}
