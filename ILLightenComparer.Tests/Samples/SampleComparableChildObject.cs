using System;

namespace ILLightenComparer.Tests.Samples
{
    public sealed class SampleComparableChildObject : SampleComparableObject, IComparable<SampleComparableChildObject>
    {
        public EnumSmall? Field;

        public int CompareTo(SampleComparableChildObject other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }

            var compare = base.CompareTo(other);
            if (compare != 0)
            {
                return compare;
            }

            return Nullable.Compare(Field, other.Field);
        }

        public override int CompareTo(SampleComparableObject other)
        {
            return CompareTo(other as SampleComparableChildObject);
        }
    }
}
