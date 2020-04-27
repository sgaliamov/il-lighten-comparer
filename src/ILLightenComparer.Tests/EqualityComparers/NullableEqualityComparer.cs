using System.Collections;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.EqualityComparers
{
    internal sealed class NullableEqualityComparer<TValue> : IEqualityComparer<TValue?>, IEqualityComparer where TValue : struct
    {
        private readonly IEqualityComparer<TValue> _valueComparer;

        public NullableEqualityComparer(IEqualityComparer<TValue> valueComparer = null) => _valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;

        bool IEqualityComparer.Equals(object x, object y) => Equals((TValue?)x, (TValue?)y);

        public bool Equals(TValue? x, TValue? y) => _valueComparer.Equals(x ?? default, y ?? default);

        public int GetHashCode(object obj) => GetHashCode((TValue?)obj);

        public int GetHashCode(TValue? obj) => _valueComparer.GetHashCode(obj ?? default);
    }
}
