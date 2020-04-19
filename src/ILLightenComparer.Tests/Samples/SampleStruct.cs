using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ILLightenComparer.Tests.EqualityComparers;
using ILLightenComparer.Tests.Utilities;

namespace ILLightenComparer.Tests.Samples
{
    [SuppressMessage("Usage", "CA2231:Overload operator equals on overriding value type Equals", Justification = "Test class")]
    [SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "Test class")]
    [System.Diagnostics.DebuggerDisplay("{ToString()}")]
    public struct SampleStruct<TMember>
    {
        public TMember Field;
        public TMember Property { get; set; }

        public override bool Equals(object obj) => obj is SampleStruct<TMember> @struct && Equals(@struct);

        public bool Equals(SampleStruct<TMember> other) =>
            EqualityComparer<TMember>.Default.Equals(Field, other.Field)
            && EqualityComparer<TMember>.Default.Equals(Property, other.Property);

        public override int GetHashCode() => HashCodeCombiner.Combine(Field, Property);

        public override string ToString()
        {
            var field = Field.ToStringEx();
            var property = Property.ToStringEx();

            return $"Struct: {{ Field: {field}, Property: {property} }}";
        }
    }
}
