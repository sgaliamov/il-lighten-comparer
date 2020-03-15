using System.Reflection;
using System.Reflection.Emit;
using Illuminator;

namespace ILLightenComparer.Extensions
{
    internal static class ILEmitterExtensions
    {
        public static ILEmitter EmitReturnIfNonZero(this ILEmitter il, Label next) =>
            il.Store(typeof(int), out var result)
              .LoadLocal(result)
              .IfFalse(next)
              .LoadLocal(result)
              .Return();
    }
}
