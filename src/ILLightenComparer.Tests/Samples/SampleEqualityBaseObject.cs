using System.Collections.Generic;
using ILLightenComparer.Tests.EqualityComparers;
using ILLightenComparer.Tests.Utilities;

namespace ILLightenComparer.Tests.Samples
{
    public class SampleEqualityBaseObject<TMember>
    {
        public TMember Field;
        public TMember Property { get; set; }

        public override bool Equals(object obj) => Equals(obj as SampleEqualityBaseObject<TMember>);

        public bool Equals(SampleEqualityBaseObject<TMember> other) =>
            other != null
            && EqualityComparer<TMember>.Default.Equals(Field, other.Field)
            && EqualityComparer<TMember>.Default.Equals(Property, other.Property);

        public override int GetHashCode() => HashCodeCombiner.Combine(Field, Property);

        public override string ToString() => this.ToJson();
    }
}
