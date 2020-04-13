using System;
using System.Reflection.Emit;
using Illuminator;

namespace ILLightenComparer.Extensions
{
    internal static class ILEmitterExtensions
    {
        private const string LengthMethodName = nameof(Array.Length);

        public static ILEmitter EmitArrayLength(this ILEmitter il, Type arrayType, LocalBuilder array, out LocalBuilder count) => il
            .LoadLocal(array)
            .Call(arrayType.GetPropertyGetter(LengthMethodName))
            .Store(typeof(int), out count);
    }
}
