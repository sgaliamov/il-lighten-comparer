using System;
using System.Collections;
using System.Collections.Generic;
using ILLightenComparer.Emit.Types;

namespace ILLightenComparer.Emit
{
    internal sealed class EqualityComparerEmitter
    {
        public EqualityComparerEmitter(TypeEmitter typeEmitter) => throw new NotImplementedException();

        public IEqualityComparer<T> Emit<T>(CompareConfiguration configuration) =>
            throw new NotImplementedException();

        internal IEqualityComparer Emit(Type type, CompareConfiguration configuration) =>
            throw new NotImplementedException();
    }
}
