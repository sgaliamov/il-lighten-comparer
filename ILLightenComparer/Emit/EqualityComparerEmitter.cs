using System;
using System.Collections;
using System.Collections.Generic;

namespace ILLightenComparer.Emit
{
    internal sealed class EqualityComparerEmitter
    {
        private readonly Context _context;

        public EqualityComparerEmitter(Context context) => _context = context;

        public IEqualityComparer<T> Emit<T>() =>
            throw new NotImplementedException();

        internal IEqualityComparer Emit(Type type) =>
            throw new NotImplementedException();
    }
}
