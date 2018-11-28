using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests
{
    public sealed class ContainerObject
    {
        public static IComparer<ContainerObject> Comparer { get; } = new ValueRelationalComparer();

        public ComparableNestedObject Comparable { get; set; }
        public NestedObject First { get; set; }
        public NestedObject Second { get; set; } // todo: test with field
        public int Value { get; set; }

        private sealed class ValueRelationalComparer : IComparer<ContainerObject>
        {
            public int Compare(ContainerObject x, ContainerObject y)
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

                var comparableComparison = Comparer<ComparableNestedObject>.Default.Compare(x.Comparable, y.Comparable);
                if (comparableComparison != 0)
                {
                    return comparableComparison;
                }

                var compare = x.Value.CompareTo(y.Value);
                if (compare != 0)
                {
                    return compare;
                }

                compare = NestedObject.Comparer.Compare(x.First, y.First);
                if (compare != 0)
                {
                    return compare;
                }

                compare = NestedObject.Comparer.Compare(x.Second, y.Second);
                if (compare != 0)
                {
                    return compare;
                }

                return 0;
            }
        }
    }
}
