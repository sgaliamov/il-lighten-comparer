using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.ComparableTests.Samples
{
    public struct ComparableStruct<TMember> : IComparable<ComparableStruct<TMember>>
    {
        public TMember Field;

        public TMember Property { get; set; }

        public int CompareTo(ComparableStruct<TMember> other)
        {
            var compare = Comparer<TMember>.Default.Compare(Field, other.Field);
            if (compare != 0)
            {
                return compare;
            }

            return Comparer<TMember>.Default.Compare(Property, other.Property);
        }
    }
}
