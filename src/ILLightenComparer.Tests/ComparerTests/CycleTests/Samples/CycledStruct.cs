using System;
using System.Collections.Generic;
using ILLightenComparer.Tests.Utilities;

namespace ILLightenComparer.Tests.ComparerTests.CycleTests.Samples
{
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
                var setX = new ConcurrentSet<object>();
                var setY = new ConcurrentSet<object>();

                var compare = Compare(x, y, setX, setY);
                if (compare != 0) {
                    return compare;
                }

                return setX.Count - setY.Count;
            }

            public static int Compare(
                CycledStruct? x,
                CycledStruct? y,
                ConcurrentSet<object> xSet,
                ConcurrentSet<object> ySet)
            {
                if (x.HasValue) {
                    if (y.HasValue) {
                        return Compare(x.Value, y.Value, xSet, ySet);
                    }

                    return 1;
                }

                if (y.HasValue) {
                    return -1;
                }

                return 0;
            }

            public static int Compare(CycledStruct x, CycledStruct y, ConcurrentSet<object> xSet, ConcurrentSet<object> ySet)
            {
                var compare = Nullable.Compare(x.Property, y.Property);
                if (compare != 0) {
                    return compare;
                }

                compare = CycledStructObject.RelationalComparer.Compare(x.FirstObject, y.FirstObject, xSet, ySet);
                if (compare != 0) {
                    return compare;
                }

                return CycledStructObject.RelationalComparer.Compare(x.SecondObject, y.SecondObject, xSet, ySet);
            }
        }
    }
}
