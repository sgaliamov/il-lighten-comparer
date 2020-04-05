using System.Reflection.Emit;
using Illuminator;

namespace ILLightenComparer.Comparer
{
    internal static class CustomEmiters
    {
        public static ILEmitter EmitReturnIfTruthy(this ILEmitter il, Label next) => il
            .Store(typeof(int), out var result)
            .LoadLocal(result)
            .IfFalse(next)
            .LoadLocal(result)
            .Return();
    }
}
