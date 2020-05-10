using System.Collections.Generic;
using ILLightenComparer.Tests.EqualityComparers;

namespace ILLightenComparer.Tests.EqualityTests.CycleTests.Samples
{
    public sealed class ThreeSealed
    {
        public OneSealed One { get; set; }

        public override bool Equals(object obj) => Equals((ThreeSealed)obj);

        public bool Equals(ThreeSealed other)
        {
            return other != null
                && EqualityComparer<OneSealed>.Default.Equals(One, other.One);
        }

        public override int GetHashCode() => HashCodeCombiner.Combine(One);
    }
}
