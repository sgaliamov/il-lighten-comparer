using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ILLightenComparer.Tests.EqualityComparers
{
    internal sealed class UglyEqualityComparer<T> : IEqualityComparer<T>, IEqualityComparer
    {
        private readonly IEqualityComparer _comparer;
        private readonly MethodInfo _staticHasher;

        public UglyEqualityComparer(IEqualityComparer comparer, MethodInfo staticHasher)
        {
            _comparer = comparer;
            _staticHasher = staticHasher;
        }

        public bool Equals(T x, T y) => _comparer.Equals(x, y);

        public new bool Equals(object x, object y) => Equals((T)x, (T)y);

        public int GetHashCode(T obj) => (int)_staticHasher.Invoke(null, new object[] { obj });

        public int GetHashCode(object obj) => GetHashCode((T)obj);
    }
}
