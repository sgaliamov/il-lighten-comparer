using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples.Cycled
{
    using Set = ConcurrentDictionary<object, byte>;

    public struct CycledStruct
    {
        public int Value { get; set; }
        public CycledStructObject FirstObject { get; set; }
        public CycledStructObject SecondObject;

        public static IComparer<CycledStruct> Comparer { get; } = new RelationalComparer();

        public sealed class RelationalComparer : IComparer<CycledStruct>
        {
            public int Compare(CycledStruct x, CycledStruct y)
            {
                var setX = new Set();
                var setY = new Set();

                var compare = Compare(x, y, setX, setY);
                if (compare != 0) { return compare; }

                return setX.Count - setY.Count;
            }

            public static int Compare(CycledStruct x, CycledStruct y, Set xSet, Set ySet)
            {
                var compare = x.Value.CompareTo(y.Value);
                if (compare != 0) { return compare; }

                compare = CycledStructObject.RelationalComparer.Compare(x.FirstObject, y.FirstObject, xSet, ySet);
                if (compare != 0) { return compare; }

                return CycledStructObject.RelationalComparer.Compare(x.SecondObject, y.SecondObject, xSet, ySet);
            }
        }
    }
}
