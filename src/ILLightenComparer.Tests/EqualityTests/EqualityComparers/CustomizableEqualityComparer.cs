using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.EqualityTests.EqualityComparers
{
    internal class CustomizableEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _comparer;
        private readonly Func<T, int> _hasher;

        public CustomizableEqualityComparer(Func<T, T, bool> comparer, Func<T, int> hasher)
        {
            _comparer = comparer;
            _hasher = hasher;
        }

        public bool Equals(T x, T y) => _comparer(x, y);

        public int GetHashCode(T obj) => _hasher(obj);
    }
}
