using System;
using System.Collections;
using System.Collections.Generic;

namespace ILLightenComparer.Emit
{
    internal sealed class EqualityComparerEmitter
    {
        private readonly EmitterContext _emitterContext;

        public EqualityComparerEmitter(EmitterContext emitterContext) => _emitterContext = emitterContext;

        public IEqualityComparer<T> Emit<T>(CompareConfiguration configuration) =>
            throw new NotImplementedException();

        internal IEqualityComparer Emit(Type type, CompareConfiguration configuration) =>
            throw new NotImplementedException();
    }
}
