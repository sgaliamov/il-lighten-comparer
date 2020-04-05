using System.Collections;

namespace ILLightenComparer.Tests.EqualityComparers
{
    internal struct HashCodeCombiner
    {
        private long _combinedHash64;

        public int CombinedHash { get { return (int)_combinedHash64; } }

        private HashCodeCombiner(long seed) => _combinedHash64 = seed;

        public static HashCodeCombiner Combine(IEqualityComparer comparer, params object[] objects)
        {
            var combiner = new HashCodeCombiner(0x1505L);

            foreach (var o in objects) {
                var hashCode = comparer?.GetHashCode(o) ?? o?.GetHashCode() ?? 0;
                combiner.Add(hashCode);
            }

            return combiner;
        }

        public static HashCodeCombiner Combine(params object[] objects) => Combine(null, objects);

        public static implicit operator int(HashCodeCombiner self) => self.CombinedHash;

        private void Add(int i) => _combinedHash64 = ((_combinedHash64 << 5) + _combinedHash64) ^ i;
    }
}
