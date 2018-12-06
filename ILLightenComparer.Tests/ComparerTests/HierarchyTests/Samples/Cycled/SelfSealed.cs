using System.Collections.Concurrent;
using System.Collections.Generic;
using ILLightenComparer.Tests.Utilities;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples.Cycled
{
    using Set = ConcurrentDictionary<object, byte>;

    public sealed class SelfSealed
    {
        public readonly int Id;
        public SelfSealed First;

        public SelfSealed() => Id = this.GetObjectId();

        public static RelationalComparer Comparer { get; } = new RelationalComparer();
        public SelfSealed Second { get; set; }
        public int Value { get; set; }

        public override string ToString() => Id.ToString();

        public sealed class RelationalComparer : IComparer<SelfSealed>
        {
            public int Compare(SelfSealed x, SelfSealed y)
            {
                var setX = new Set();
                var setY = new Set();

                var compare = Compare(x, y, setX, setY);
                if (compare != 0) { return compare; }

                return setX.Count - setY.Count;
            }

            private static int Compare(SelfSealed x, SelfSealed y, Set xSet, Set ySet)
            {
                if (ReferenceEquals(x, y)) { return 0; }

                if (ReferenceEquals(null, y)) { return 1; }

                if (ReferenceEquals(null, x)) { return -1; }

                if (!xSet.TryAdd(x, 0) & !ySet.TryAdd(y, 0)) { return 0; }

                var compareFirst = Compare(x.First, y.First, xSet, ySet);
                if (compareFirst != 0) { return compareFirst; }

                var compareSecond = Compare(x.Second, y.Second, xSet, ySet);
                if (compareSecond != 0) { return compareSecond; }

                var compareValue = x.Value.CompareTo(y.Value);
                if (compareValue != 0) { return compareValue; }

                return 0;
            }
        }
    }
}
