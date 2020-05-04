using System.Reflection.Emit;
using Illuminator;
using static Illuminator.Functional;

namespace ILLightenComparer.Abstractions
{
    internal interface IHasherEmitter
    {
        /// <summary>
        /// Hashing logic builder.
        /// </summary>
        ILEmitter Emit(ILEmitter il);

        /// <summary>
        /// Hashing logic builder with variable to accumulate hash.
        /// </summary>
        ILEmitter Emit(ILEmitter il, LocalBuilder hash);
    }

    internal static class HasherEmitterExtensions
    {
        public static ILEmitterFunc Emit(this IHasherEmitter hasher, LocalBuilder hash) => il => hasher.Emit(il, hash);

        public static ILEmitter EmitHashing(this IHasherEmitter hasher, ILEmitter il, LocalBuilder hash)
        {
            var add = Add(
                ShiftLeft(LoadLocal(hash), LoadInteger(5)),
                LoadLocal(hash));

            return il
                .Xor(add, hasher.Emit(hash))
                .Store(hash);
        }
    }
}
