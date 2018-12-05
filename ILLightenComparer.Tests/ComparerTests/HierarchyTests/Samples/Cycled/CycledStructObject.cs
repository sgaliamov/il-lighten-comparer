using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples.Cycled
{
    public sealed class CycledStructObject
    {
        public CycledStruct FirstStruct;
        public string Text;
        public static IComparer<CycledStructObject> Comparer { get; } = new RelationalComparer();
        public CycledStruct SecondStruct { get; set; } // todo: test with nullable

        public sealed class RelationalComparer : IComparer<CycledStructObject>
        {
            public int Compare(CycledStructObject x, CycledStructObject y)
            {
                var setX = new HashSet<object>();
                var setY = new HashSet<object>();

                var compare = Compare(x, y, setX, setY);
                if (compare != 0) { return compare; }

                return setX.Count - setY.Count;
            }

            public static int Compare(
                CycledStructObject x,
                CycledStructObject y,
                ISet<object> xSet,
                ISet<object> ySet)
            {
                if (ReferenceEquals(x, y))
                {
                    return 0;
                }

                if (ReferenceEquals(null, y))
                {
                    return 1;
                }

                if (ReferenceEquals(null, x))
                {
                    return -1;
                }

                if (xSet.Contains(x) && ySet.Contains(y)) { return 0; }

                xSet.Add(x);
                ySet.Add(y);

                var compare = string.Compare(x.Text, y.Text, StringComparison.Ordinal);
                if (compare != 0) { return compare; }

                compare = CycledStruct.RelationalComparer.Compare(x.FirstStruct, y.FirstStruct, xSet, ySet);
                if (compare != 0) { return compare; }

                return CycledStruct.RelationalComparer.Compare(x.SecondStruct, y.SecondStruct, xSet, ySet);
            }
        }
    }
}
