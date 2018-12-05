using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples.Cycled
{
    public sealed class CycledStructObject
    {
        public string Text;
        public static IComparer<CycledStructObject> Comparer { get; } = new RelationalComparer();
        public CycledStruct Struct { get; set; } // todo: test with nullable

        private sealed class RelationalComparer : IComparer<CycledStructObject>
        {
            public int Compare(CycledStructObject x, CycledStructObject y)
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

                var compare = string.Compare(x.Text, y.Text, StringComparison.Ordinal);
                if (compare != 0) { return compare; }

                compare = CycledStruct.Comparer.Compare(x.Struct, y.Struct);
                if (compare != 0) { return compare; }

                return compare;
            }
        }
    }
}
