using System;
using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.ComparerTests.ComparableTests.Samples
{
    public sealed class ChildComparableObject : ComparableObject, IComparable<ChildComparableObject>
    {
        public EnumSmall? Field;

        public int CompareTo(ChildComparableObject other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }

            var comparableObjectComparison = base.CompareTo(other);
            if (comparableObjectComparison != 0)
            {
                return comparableObjectComparison;
            }

            return Nullable.Compare(Field, other.Field);
        }
    }
}
