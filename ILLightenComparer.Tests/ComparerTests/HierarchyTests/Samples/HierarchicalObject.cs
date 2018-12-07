using System.Collections.Generic;
using ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples.Nested;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples
{
    public sealed class HierarchicalObject
    {
        public ComparableObject ComparableField;
        public SealedNestedObject NestedField;
        public NestedStruct NestedStructField;

        public static IComparer<HierarchicalObject> Comparer { get; } = new ValueRelationalComparer();

        public ComparableObject ComparableProperty { get; set; }
        public SealedNestedObject FirstProperty { get; set; }
        public NestedStruct? NestedNullableStructProperty { get; set; }
        public SealedNestedObject SecondProperty { get; set; }
        public int Value { get; set; }

        private sealed class ValueRelationalComparer : IComparer<HierarchicalObject>
        {
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

                var compare = Comparer<ComparableObject>.Default.Compare(
                    x.ComparableProperty,
                    y.ComparableProperty);
                if (compare != 0)
                {
                    return compare;
                }

                compare = Comparer<ComparableObject>.Default.Compare(
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

                compare = Comparer<SealedNestedObject>.Default.Compare(x.FirstProperty, y.FirstProperty);
                if (compare != 0)
                {
                    return compare;
                }

                compare = Comparer<SealedNestedObject>.Default.Compare(x.SecondProperty, y.SecondProperty);
                if (compare != 0)
                {
                    return compare;
                }

                compare = Comparer<SealedNestedObject>.Default.Compare(x.NestedField, y.NestedField);
                if (compare != 0)
                {
                    return compare;
                }

                compare = NestedStruct.Comparer.Compare(
                    x.NestedStructField,
                    y.NestedStructField);
                if (compare != 0)
                {
                    return compare;
                }

                compare = NestedStruct.Comparer.Compare(
                    x.NestedNullableStructProperty,
                    y.NestedNullableStructProperty);
                if (compare != 0)
                {
                    return compare;
                }

                return 0;
            }
        }
    }
}
