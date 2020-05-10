using System.Collections.Generic;
using ILLightenComparer.Tests.EqualityComparers;

namespace ILLightenComparer.Tests.EqualityTests.CycleTests.Samples
{
    public sealed class OneSealed
    {
        public TwoSealed Two { get; set; }
        public sbyte Value { get; set; }

        public override bool Equals(object obj) => Equals((OneSealed)obj);

        public bool Equals(OneSealed other)
        {
            return other != null
                && EqualityComparer<TwoSealed>.Default.Equals(Two, other.Two)
                && Value == other.Value;
        }

        public override int GetHashCode() => HashCodeCombiner.Combine<object>(Two, Value);
    }
}
