using System;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests
{
    public sealed class ComparableNestedObject : IComparable<ComparableNestedObject>
    {
        public int Value { get; set; }

        public int CompareTo(ComparableNestedObject other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }

            return Value.CompareTo(other.Value);
        }
    }
}
