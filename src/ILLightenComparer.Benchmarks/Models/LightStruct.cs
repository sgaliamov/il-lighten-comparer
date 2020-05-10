using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ILLightenComparer.Tests.Comparers;

namespace ILLightenComparer.Benchmarks.Models
{
    [SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "<Pending>")]
    public struct LightStruct
    {
        public byte Key;
        public char[] Value;
    }

    internal sealed class LightStructComparer : IComparer<LightStruct>
    {
        public static IComparer<LightStruct> Instance { get; } = new LightStructComparer();

        private readonly CollectionComparer<char> _collectionComparer = new CollectionComparer<char>();

        public int Compare(LightStruct x, LightStruct y)
        {
            var compare = x.Key.CompareTo(y.Key);
            if (compare != 0) {
                return compare;
            }

            return _collectionComparer.Compare(x.Value, y.Value);
        }
    }
}
