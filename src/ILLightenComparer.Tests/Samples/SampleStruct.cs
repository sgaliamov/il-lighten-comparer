using System.Collections.Generic;
using ILLightenComparer.Tests.EqualityTests.EqualityComparers;

namespace ILLightenComparer.Tests.Samples
{
    public struct SampleStruct<TMember>
    {
        public TMember Field;
        public TMember Property { get; set; }

        public override bool Equals(object obj) => obj is SampleStruct<TMember> @struct && Equals(@struct);

        public bool Equals(SampleStruct<TMember> other) => EqualityComparer<TMember>.Default.Equals(Field, other.Field) && EqualityComparer<TMember>.Default.Equals(Property, other.Property);

        public override int GetHashCode() => HashCodeCombiner.Combine(Field, Property);

        public static bool operator ==(SampleStruct<TMember> left, SampleStruct<TMember> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SampleStruct<TMember> left, SampleStruct<TMember> right)
        {
            return !(left == right);
        }
    }
}
