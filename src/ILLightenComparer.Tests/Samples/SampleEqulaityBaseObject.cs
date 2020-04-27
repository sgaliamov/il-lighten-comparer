using System.Collections.Generic;
using ILLightenComparer.Tests.EqualityComparers;

namespace ILLightenComparer.Tests.Samples
{
    public class SampleEqulaityBaseObject<TMember>
    {
        public TMember Field;
        public TMember Property { get; set; }

        public override bool Equals(object obj) => Equals(obj as SampleEqulaityBaseObject<TMember>);

        public bool Equals(SampleEqulaityBaseObject<TMember> other) =>
            other != null
            && EqualityComparer<TMember>.Default.Equals(Field, other.Field)
            && EqualityComparer<TMember>.Default.Equals(Property, other.Property);

        public override int GetHashCode() => HashCodeCombiner.Combine(Field, Property);

        public override string ToString() => $"{{ {Field}, {Property} }}";
    }
}
