using System;

namespace ILLightenComparer.Tests.Samples
{
    public class SampleComparableObject : IComparable<SampleComparableObject>
    {
        public static bool UsedCompareTo { get; private set; }

        public int Property { get; set; }

        public virtual int CompareTo(SampleComparableObject other)
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
