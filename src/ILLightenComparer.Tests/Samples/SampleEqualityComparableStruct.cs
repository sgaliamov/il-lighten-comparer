using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ILLightenComparer.Tests.EqualityComparers;

namespace ILLightenComparer.Tests.Samples
{
    [SuppressMessage("Usage", "CA2231:Overload operator equals on overriding value type Equals", Justification = "<Pending>")]
    public struct SampleEqualityComparableStruct<TMember>
    {
        public static IEqualityComparer<TMember> Comparer = EqualityComparer<TMember>.Default;

        public TMember Field;
        public TMember Property { get; set; }

        public override bool Equals(object obj) => obj is SampleEqualityComparableStruct<TMember> @struct && Equals(@struct);

        public bool Equals(SampleEqualityComparableStruct<TMember> other) =>
            EqualityComparer<TMember>.Default.Equals(Field, other.Field)
            && EqualityComparer<TMember>.Default.Equals(Property, other.Property);

        public override int GetHashCode() => HashCodeCombiner.Combine(Field, Property);

        public override string ToString() => $"{{ {Field}, {Property} }}";
    }
}
