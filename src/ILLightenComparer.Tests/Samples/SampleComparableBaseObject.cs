using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ILLightenComparer.Tests.Samples
{
    [SuppressMessage("Design", "CA1036:Override methods on comparable types", Justification = "Test class")]
    public class SampleComparableBaseObject<TMember> : IComparable<SampleComparableBaseObject<TMember>>
    {
        [SuppressMessage("Design", "RCS1158:Static member in generic type should use a type parameter.", Justification = "Test class")]
        public static bool UsedCompareTo;
        public static IComparer<TMember> Comparer = Comparer<TMember>.Default;

        public TMember Field;
        public TMember Property { get; set; }

        public virtual int CompareTo(SampleComparableBaseObject<TMember> other)
        {
            UsedCompareTo = true;

            if (ReferenceEquals(this, other)) {
                return 0;
            }

            if (other is null) {
                return 1;
            }

            var compare = Comparer.Compare(Field, other.Field);
            if (compare != 0) {
                return compare;
            }

            return Comparer.Compare(Property, other.Property);
        }

        public override string ToString() => $"{{ {Field}, {Property} }}";
    }
}
