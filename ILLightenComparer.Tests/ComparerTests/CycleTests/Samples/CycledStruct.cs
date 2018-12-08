using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.CycleTests.Samples
{
    using Set = ConcurrentDictionary<object, byte>;

    public struct CycledStruct
    {
        public sbyte? Property { get; set; }
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
                if (compare != 0)
                {
                    return compare;
                }

                return setX.Count - setY.Count;
            }

            public static int Compare(CycledStruct? x, CycledStruct? y, Set xSet, Set ySet)
            {
                if (x.HasValue)
                {
                    if (y.HasValue)
                    {
                        return Compare(x.Value, y.Value, xSet, ySet);
                    }

                    return 1;
                }

                if (y.HasValue)
                {
                    return -1;
                }

                return 0;
            }

            public static int Compare(CycledStruct x, CycledStruct y, Set xSet, Set ySet)
            {
                var compare = Nullable.Compare(x.Property, y.Property);
                if (compare != 0)
                {
                    return compare;
                }

                compare = CycledStructObject.RelationalComparer.Compare(x.FirstObject, y.FirstObject, xSet, ySet);
                if (compare != 0)
                {
                    return compare;
                }

                return CycledStructObject.RelationalComparer.Compare(x.SecondObject, y.SecondObject, xSet, ySet);
            }
        }
    }
}
