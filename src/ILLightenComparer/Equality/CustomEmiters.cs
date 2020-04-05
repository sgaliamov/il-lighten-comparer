using System.Reflection.Emit;
using Illuminator;

namespace ILLightenComparer.Equality
{
    internal static class CustomEmiters
    {
        public static ILEmitter EmitReturnIfFalsy(this ILEmitter il, Label next) => il.IfTrue(next).Return(0);
    }
}
