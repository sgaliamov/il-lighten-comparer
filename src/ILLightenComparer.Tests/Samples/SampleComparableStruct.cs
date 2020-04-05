using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.Samples
{
    public struct SampleComparableStruct<TMember> : IComparable<SampleComparableStruct<TMember>>
    {
        public static IComparer<TMember> Comparer = Comparer<TMember>.Default;

        public TMember Field;
        public TMember Property { get; set; }

        public int CompareTo(SampleComparableStruct<TMember> other)
        {
            var compare = Comparer.Compare(Field, other.Field);
            if (compare != 0) {
                return compare;
            }

            return Comparer.Compare(Property, other.Property);
        }

        public override string ToString() => $"{{ {Field}, {Property} }}";
    }
}
