using System;
using System.Reflection.Emit;
using Illuminator;
using static Illuminator.Functional;

namespace ILLightenComparer.Abstractions
{
    internal interface IHasherEmitter
    {
        ILEmitter Emit(ILEmitter il);
        ILEmitter Emit(ILEmitter il, LocalBuilder hash);
    }

    internal static class HasherEmitterExtensions
    {
        public static Func<ILEmitter, ILEmitter> Emit(this IHasherEmitter hasher, LocalBuilder hash) => il => hasher.Emit(il, hash);

        public static ILEmitter EmitHashing(this IHasherEmitter hasher, ILEmitter il, LocalBuilder hash)
        {
            var add = Add(
                ShiftLeft(LoadLocal(hash), LoadInteger(5)),
                LoadLocal(hash));

            return il
                .Xor(add, Execute(hasher.Emit(hash), Cast(typeof(long))))
                .Store(hash);
        }
    }
}
