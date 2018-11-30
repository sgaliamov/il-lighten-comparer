using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests
{
    public sealed class ContainerObject
    {
        public NestedObject NestedField;

        public static IComparer<ContainerObject> Comparer { get; } = new ValueRelationalComparer();

        public ComparableNestedObject ComparableProperty { get; set; }
        public ComparableNestedObject ComparableField;
        public NestedObject FirstProperty { get; set; }
        public NestedObject SecondProperty { get; set; }
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

                var compare = Comparer<ComparableNestedObject>.Default.Compare(
                    x.ComparableProperty,
                    y.ComparableProperty);
                if (compare != 0)
                {
                    return compare;
                }

                compare = Comparer<ComparableNestedObject>.Default.Compare(
                    x.ComparableField,
                    y.ComparableField);
                if (compare != 0)
                {
                    return compare;
                }

                compare = x.Value.CompareTo(y.Value);
                if (compare != 0)
                {
                    return compare;
                }

                compare = NestedObject.Comparer.Compare(x.FirstProperty, y.FirstProperty);
                if (compare != 0)
                {
                    return compare;
                }

                compare = NestedObject.Comparer.Compare(x.SecondProperty, y.SecondProperty);
                if (compare != 0)
                {
                    return compare;
                }

                compare = NestedObject.Comparer.Compare(x.NestedField, y.NestedField);
                if (compare != 0)
                {
                    return compare;
                }

                return 0;
            }
        }
    }
}
