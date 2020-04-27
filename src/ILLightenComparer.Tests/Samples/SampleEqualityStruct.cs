using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ILLightenComparer.Tests.EqualityComparers;
using ILLightenComparer.Tests.Utilities;

namespace ILLightenComparer.Tests.Samples
{
    [SuppressMessage("Usage", "CA2231:Overload operator equals on overriding value type Equals", Justification = "Test class")]
    [SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "Test class")]
    [DebuggerDisplay("{ToString()}")]
    public struct SampleEqualityStruct<TMember>
    {
        public static IEqualityComparer<TMember> Comparer = EqualityComparer<TMember>.Default;

        public TMember Field;
        public TMember Property { get; set; }

        public override bool Equals(object obj) => Equals((SampleEqualityStruct<TMember>)obj);

        public bool Equals(SampleEqualityStruct<TMember> other) =>
            Comparer.Equals(Field, other.Field)
            && Comparer.Equals(Property, other.Property);

        public override int GetHashCode() => HashCodeCombiner.Combine(Field, Property);

        public override string ToString() => $"Struct: {this.ToJson()}";
    }
}
