using System.Collections.Generic;
using ILLightenComparer.Tests.Samples.Comparers;

namespace ILLightenComparer.Benchmarks.Benchmark
{
    public struct LightStruct
    {
        public byte Key;
        public char[] Value;

        private sealed class RelationalComparer : IComparer<LightStruct>
        {
            private readonly CollectionComparer<LightStruct> _collectionComparer = new CollectionComparer<LightStruct>();

            public int Compare(LightStruct x, LightStruct y)
            {
                var compare = x.Key.CompareTo(y.Key);
                if (compare != 0)
                {
                    return compare;
                }

                return _collectionComparer.Compare(x.Value, y.Value);
            }
        }

        public static IComparer<LightStruct> Comparer { get; } = new RelationalComparer();
    }
}
