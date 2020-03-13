using System;
using System.Reflection.Emit;
using ILLightenComparer.Shared;
using Illuminator;

namespace ILLightenComparer.Equality.Comparisons
{
    internal sealed class CustomHasher : IStepEmitter
    {
        public bool PutsResultInStack { get; }

        public ILEmitter Emit(ILEmitter il, Label gotoNext)
        {
            throw new NotImplementedException();
        }

        public ILEmitter Emit(ILEmitter il) => Emit(il, default).Return();
    }
}
