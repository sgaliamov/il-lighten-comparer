using System;
using System.Collections;
using System.Collections.Generic;

namespace ILLightenComparer.Emit
{
    internal sealed class EqualityComparerBuilder
    {
        private readonly Context _context;

        public EqualityComparerBuilder(Context context) => _context = context;

        public IEqualityComparer<T> Build<T>() =>
            throw new NotImplementedException();

        internal IEqualityComparer Build(Type type) =>
            throw new NotImplementedException();
    }
}
