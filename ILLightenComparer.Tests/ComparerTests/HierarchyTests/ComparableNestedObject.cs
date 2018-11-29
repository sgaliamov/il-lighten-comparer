using System;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests
{
    public sealed class ComparableNestedObject : IComparable<ComparableNestedObject>
    {
        public static bool UsedCompareTo { get; private set; }
        public int Value { get; set; }

        public int CompareTo(ComparableNestedObject other)
        {
            UsedCompareTo = true;

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
