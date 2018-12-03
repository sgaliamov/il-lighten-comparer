using System;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples
{
    public sealed class NestedObject : BaseNestedObject
    {
        public DeepNestedObject DeepNestedField;
        public DeepNestedObject DeepNestedProperty { get; set; }

        public override int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return 1;
            }

            if (ReferenceEquals(this, obj))
            {
                return 0;
            }

            return obj is NestedObject other
                ? CompareTo(other)
                : throw new ArgumentException($"Object must be of type {nameof(NestedObject)}.");
        }

        private int CompareTo(NestedObject other)
        {
            var compare = DeepNestedObject.Comparer.Compare(DeepNestedField, other.DeepNestedField);
            if (compare != 0)
            {
                return compare;
            }

            compare = DeepNestedObject.Comparer.Compare(DeepNestedProperty, other.DeepNestedProperty);
            if (compare != 0)
            {
                return compare;
            }

            return base.CompareTo(other);
        }
    }
}
