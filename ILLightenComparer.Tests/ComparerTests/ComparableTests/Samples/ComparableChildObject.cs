using System;
using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.ComparerTests.ComparableTests.Samples
{
    public sealed class ComparableChildObject : ComparableObject, IComparable<ComparableChildObject>
    {
        public EnumSmall? Field;

        public int CompareTo(ComparableChildObject other)
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

        public override int CompareTo(ComparableObject other) => CompareTo(other as ComparableChildObject);
    }
}
