using System.Collections.Generic;
using ILLightenComparer.Tests.EqualityComparers;

namespace ILLightenComparer.Tests.Samples
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2231:Overload operator equals on overriding value type Equals", Justification = "<Pending>")]
    public struct SampleStruct<TMember>
    {
        public TMember Field;
        public TMember Property { get; set; }

        public override bool Equals(object obj) => obj is SampleStruct<TMember> @struct && Equals(@struct);

        public bool Equals(SampleStruct<TMember> other) =>
            EqualityComparer<TMember>.Default.Equals(Field, other.Field)
            && EqualityComparer<TMember>.Default.Equals(Property, other.Property);

        public override int GetHashCode() => HashCodeCombiner.Combine(Field, Property);
    }
}
