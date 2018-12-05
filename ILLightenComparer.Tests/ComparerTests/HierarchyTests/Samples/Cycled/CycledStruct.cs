using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples.Cycled
{
    public struct CycledStruct
    {
        public int Value { get; set; }
        public static IComparer<CycledStruct> Comparer { get; } = new RelationalComparer();
        public CycledStructObject Object { get; set; }

        private sealed class RelationalComparer : IComparer<CycledStruct>
        {
            public int Compare(CycledStruct x, CycledStruct y)
            {
                var compare = x.Value.CompareTo(y.Value);
                if (compare != 0) { return compare; }

                compare = CycledStructObject.Comparer.Compare(x.Object, y.Object);
                if (compare != 0) { return compare; }

                return 0;
            }
        }
    }
}
