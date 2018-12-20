using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.Samples
{
    public struct SampleComparableStruct<TMember> : IComparable<SampleComparableStruct<TMember>>
    {
        public TMember Field;
        public TMember Property { get; set; }

        static SampleComparableStruct()
        {
            Comparer = typeof(TMember) == typeof(string)
                           ? (IComparer<TMember>)StringComparer.Ordinal
                           : Comparer<TMember>.Default;
        }

        private static readonly IComparer<TMember> Comparer;

        public int CompareTo(SampleComparableStruct<TMember> other)
        {
            var compare = Comparer.Compare(Field, other.Field);
            if (compare != 0)
            {
                return compare;
            }

            return Comparer.Compare(Property, other.Property);
        }
    }
}
