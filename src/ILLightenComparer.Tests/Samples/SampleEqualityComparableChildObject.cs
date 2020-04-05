using System;
using System.Collections.Generic;
using ILLightenComparer.Tests.EqualityComparers;

namespace ILLightenComparer.Tests.Samples
{
    public sealed class SampleEqualityComparableChildObject<TMember> : SampleEqualityComparableBaseObject<TMember>, IEquatable<SampleEqualityComparableChildObject<TMember>>
    {
        public static IEqualityComparer<TMember> ChildComparer = EqualityComparer<TMember>.Default;

        public TMember ChildField;
        public TMember ChildProperty { get; set; }

        public override bool Equals(object obj) => Equals(obj as SampleEqualityComparableChildObject<TMember>);

        public bool Equals(SampleEqualityComparableChildObject<TMember> other) =>
            other != null
            && base.Equals(other)
            && EqualityComparer<TMember>.Default.Equals(ChildField, other.ChildField)
            && EqualityComparer<TMember>.Default.Equals(ChildProperty, other.ChildProperty);

        public override int GetHashCode() => HashCodeCombiner.Combine(base.GetHashCode(), ChildField, ChildProperty);

        public override string ToString() => $"{{ {Field}, {Property}, {ChildField}, {ChildProperty} }}";
    }
}
