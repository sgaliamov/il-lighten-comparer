using System;

namespace ILLightenComparer.Tests.ComparerTests.ComparableTests.Samples
{
    public class ComparableObject : IComparable<ComparableObject>
    {
        public static bool UsedCompareTo { get; private set; }

        public int Property { get; set; }

        public int CompareTo(ComparableObject other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }

            UsedCompareTo = true;

            return Property.CompareTo(other.Property);
        }
    }
}
