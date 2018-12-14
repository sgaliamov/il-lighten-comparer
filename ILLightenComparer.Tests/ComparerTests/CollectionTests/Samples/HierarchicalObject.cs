using System.Collections;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.CollectionTests.Samples
{
    public sealed class HierarchicalObject
    {
        public static RelationalComparer Comparer { get; } = new RelationalComparer();

        public NestedObject Nested { get; set; }

        public sealed class RelationalComparer : IComparer<HierarchicalObject>, IComparer
        {
            public int Compare(object x, object y)
            {
                return Compare(x as HierarchicalObject, y as HierarchicalObject);
            }

            public int Compare(HierarchicalObject x, HierarchicalObject y)
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

                return NestedObject.Comparer.Compare(x.Nested, y.Nested);
            }
        }
    }
}
