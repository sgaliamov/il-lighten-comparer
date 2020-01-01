using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.Samples
{
    public sealed class SampleComparableChildObject<TMember> :
        SampleComparableBaseObject<TMember>,
        IComparable<SampleComparableChildObject<TMember>>
    {
        public static IComparer<TMember> ChildComparer = Comparer<TMember>.Default;

        public TMember ChildField;
        public TMember ChildProperty { get; set; }

        public int CompareTo(SampleComparableChildObject<TMember> other) {
            if (ReferenceEquals(this, other)) {
                return 0;
            }

            if (ReferenceEquals(null, other)) {
                return 1;
            }

            var compare = base.CompareTo(other);
            if (compare != 0) {
                return compare;
            }

            compare = ChildComparer.Compare(ChildField, other.ChildField);
            if (compare != 0) {
                return compare;
            }

            return ChildComparer.Compare(ChildProperty, other.ChildProperty);
        }

        public override string ToString() => $"{{ {Field}, {Property}, {ChildField}, {ChildProperty} }}";

        public override int CompareTo(SampleComparableBaseObject<TMember> other) => CompareTo(other as SampleComparableChildObject<TMember>);
    }
}
