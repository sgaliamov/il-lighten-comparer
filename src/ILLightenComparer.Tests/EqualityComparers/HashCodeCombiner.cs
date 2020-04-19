using System;
using System.Collections;
using ILLightenComparer.Tests.Utilities;

namespace ILLightenComparer.Tests.EqualityComparers
{
    [System.Diagnostics.DebuggerDisplay("{_combinedHash64}")]
    internal sealed class HashCodeCombiner
    {
        public const long Seed = 0x1505L;

        private long _combinedHash64;

        public int CombinedHash { get { return (int)_combinedHash64; } }

        private HashCodeCombiner(long seed) => _combinedHash64 = seed;

        public static HashCodeCombiner Start(long seed = Seed) => new HashCodeCombiner(seed);

        public HashCodeCombiner Combine(IEqualityComparer itemComparer, object[] objects)
        {
            if (objects is null) {
                Add(() => 0);
                return this;
            }

            foreach (var o in objects) {
                Add(() => o is null ? 0 : itemComparer?.GetHashCode(o) ?? o?.GetHashCode() ?? 0);
            }

            return this;
        }

        public HashCodeCombiner CombineObjects(params object[] objects) => Combine(null, objects.UnfoldArrays());

        public static HashCodeCombiner Combine(params object[] objects) => Start().Combine(null, objects.UnfoldArrays());

        public static implicit operator int(HashCodeCombiner self) => self.CombinedHash;

        private void Add(Func<int> hasher) => _combinedHash64 = ((_combinedHash64 << 5) + _combinedHash64) ^ hasher();
    }
}
