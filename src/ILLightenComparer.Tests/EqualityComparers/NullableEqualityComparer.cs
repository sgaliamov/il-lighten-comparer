using System.Collections;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.EqualityTests.EqualityComparers
{
    internal sealed class NullableEqualityComparer<TValue> : IEqualityComparer<TValue?>, IEqualityComparer where TValue : struct
    {
        private readonly IEqualityComparer<TValue> _valueComparer;

        public NullableEqualityComparer(IEqualityComparer<TValue> valueComparer = null) => _valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;

        bool IEqualityComparer.Equals(object x, object y) => Equals(x as TValue?, y as TValue?);

        public bool Equals(TValue? x, TValue? y) => _valueComparer.Equals(x ?? default, y ?? default);

        public int GetHashCode(object obj) => GetHashCode(obj as TValue?);

        public int GetHashCode(TValue? obj) => _valueComparer.GetHashCode(obj ?? default);
    }
}
