using System;
using System.Reflection.Emit;
using Illuminator;

namespace ILLightenComparer.Abstractions
{
    internal interface IComparisonEmitter
    {
        /// <summary>
        ///     Compare and leave a result in the stack.
        /// </summary>
        ILEmitter Emit(ILEmitter il, Label next);

        /// <summary>
        ///     Compare and return.
        /// </summary>
        ILEmitter Emit(ILEmitter il);

        /// <summary>
        ///     Does the comparison puts a result into the stack.
        ///     Comparisons with many items does not do it.
        /// </summary>
        ILEmitter EmitCheckForIntermediateResult(ILEmitter il, Label next);
    }

    internal static class ComparisonEmitterExtensions
    {
        public static Func<ILEmitter, ILEmitter> Emit(this IComparisonEmitter emitter, Label next) => il => emitter.Emit(il, next);
        public static Func<ILEmitter, ILEmitter> EmitCheckForIntermediateResult(this IComparisonEmitter emitter, Label next) => il => emitter.EmitCheckForIntermediateResult(il, next);
    }
}
