using System;
using System.Collections;
using System.Collections.Generic;
using ILLightenComparer.Emit.Types;

namespace ILLightenComparer.Emit
{
    internal sealed class EqualityComparerEmitter
    {
        private readonly TypeEmitter _emitter;

        public EqualityComparerEmitter(TypeEmitter emitter) => _emitter = emitter;

        public IEqualityComparer<T> Emit<T>(CompareConfiguration configuration) =>
            throw new NotImplementedException();

        internal IEqualityComparer Emit(Type type, CompareConfiguration configuration) =>
            throw new NotImplementedException();
    }
}
