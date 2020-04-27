using System.Collections.Generic;
using ILLightenComparer.Tests.EqualityComparers;

namespace ILLightenComparer.Tests.Samples
{
    public sealed class SampleEqualityChildObject<TMember> : SampleEqualityBaseObject<TMember>
    {
        public static IEqualityComparer<TMember> ChildComparer = EqualityComparer<TMember>.Default;

        public TMember ChildField;
        public TMember ChildProperty { get; set; }

        public override bool Equals(object obj) => Equals(obj as SampleEqualityChildObject<TMember>);

        public bool Equals(SampleEqualityChildObject<TMember> other) =>
            other != null
            && base.Equals(other)
            && EqualityComparer<TMember>.Default.Equals(ChildField, other.ChildField)
            && EqualityComparer<TMember>.Default.Equals(ChildProperty, other.ChildProperty);

        public override int GetHashCode() => HashCodeCombiner.Combine(ChildField, ChildProperty, Field, Property);

        public override string ToString() => $"{{ {Field}, {Property}, {ChildField}, {ChildProperty} }}";
    }
}
