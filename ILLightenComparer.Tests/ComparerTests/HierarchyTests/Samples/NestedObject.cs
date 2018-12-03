using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples
{
    public sealed class NestedObject : BaseNestedObject
    {
        public DeepNestedObject DeepNestedField;

        public new static IComparer<NestedObject> Comparer { get; } = new NestedObjectComparer();

        public DeepNestedObject DeepNestedProperty { get; set; }

        public override int CompareTo(object obj) => Comparer.Compare(this, obj as NestedObject);

        private sealed class NestedObjectComparer : IComparer<NestedObject>
        {
            public int Compare(NestedObject x, NestedObject y)
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

                var compare = DeepNestedObject.Comparer.Compare(x.DeepNestedField, y.DeepNestedField);
                if (compare != 0)
                {
                    return compare;
                }

                compare = DeepNestedObject.Comparer.Compare(x.DeepNestedProperty, y.DeepNestedProperty);
                if (compare != 0)
                {
                    return compare;
                }

                return BaseNestedObject.Comparer.Compare(x, y);
            }
        }
    }
}
