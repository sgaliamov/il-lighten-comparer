using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.Samples
{
    public class SampleComparableBaseObject<TMember> : IComparable<SampleComparableBaseObject<TMember>>
    {
        public static IComparer<TMember> Comparer = Comparer<TMember>.Default;

        // ReSharper disable once StaticMemberInGenericType
        public static bool UsedCompareTo;

        public TMember Field;
        public TMember Property { get; set; }

        public virtual int CompareTo(SampleComparableBaseObject<TMember> other) {
            UsedCompareTo = true;

            if (ReferenceEquals(this, other)) {
                return 0;
            }

            if (ReferenceEquals(null, other)) {
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
