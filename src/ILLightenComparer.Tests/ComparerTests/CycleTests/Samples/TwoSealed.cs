using System.Collections.Generic;
using ILLightenComparer.Tests.EqualityComparers;

namespace ILLightenComparer.Tests.ComparerTests.CycleTests.Samples
{
    public sealed class TwoSealed
    {
        public ThreeSealed Three;

        public override bool Equals(object obj) => Equals(obj as TwoSealed);

        public bool Equals(TwoSealed other)
        {
            return other != null
                && EqualityComparer<ThreeSealed>.Default.Equals(Three, other.Three);
        }

        public override int GetHashCode() => HashCodeCombiner.Combine(Three);
    }
}
