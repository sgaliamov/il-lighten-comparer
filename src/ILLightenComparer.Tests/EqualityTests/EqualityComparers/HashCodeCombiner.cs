namespace ILLightenComparer.Tests.EqualityTests.EqualityComparers
{
    internal struct HashCodeCombiner
    {
        private long _combinedHash64;

        public int CombinedHash { get { return (int)_combinedHash64; } }

        private HashCodeCombiner(long seed) => _combinedHash64 = seed;

        public HashCodeCombiner Combine(params object[] objects)
        {
            foreach (var o in objects) {
                var hashCode = o?.GetHashCode() ?? 0;
                Add(hashCode);
            }

            return this;
        }

        public static implicit operator int(HashCodeCombiner self) => self.CombinedHash;

        public void Add(int i) => _combinedHash64 = ((_combinedHash64 << 5) + _combinedHash64) ^ i;

        public static HashCodeCombiner Start() => new HashCodeCombiner(0x1505L);
    }
}
