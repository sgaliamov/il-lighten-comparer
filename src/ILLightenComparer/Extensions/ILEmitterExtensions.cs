using System.Reflection.Emit;
using Illuminator;

namespace ILLightenComparer.Extensions
{
    internal static class ILEmitterExtensions
    {
        /// <summary>
        ///     Returns value from stack if it's non zero, null or false.
        /// </summary>
        public static ILEmitter EmitReturnIfTruthy(this ILEmitter il, Label next) =>
            il.Store(typeof(int), out var result)
              .LoadLocal(result)
              .IfFalse(next)
              .LoadLocal(result)
              .Return();

        /// <summary>
        ///     Returns value from stack if it's zero, non null or true.
        /// </summary>
        public static ILEmitter EmitReturnIfFalsy(this ILEmitter il, Label next) =>
            il.Store(typeof(int), out var result)
              .LoadLocal(result)
              .IfTrue(next)
              .LoadLocal(result)
              .Return();
    }
}
