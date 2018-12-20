using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.Samples
{
    public sealed class SampleComparableChildObject<TMember> :
        SampleComparableBaseObject<TMember>,
        IComparable<SampleComparableChildObject<TMember>>
    {
        public readonly IComparer<TMember> ChildComparer = Comparer<TMember>.Default;

        public TMember ChildField;
        public TMember ChildProperty { get; set; }

        public int CompareTo(SampleComparableChildObject<TMember> other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }

            var compare = base.CompareTo(other);
            if (compare != 0)
            {
                return compare;
            }

            compare = Comparer.Compare(ChildField, other.ChildField);
            if (compare != 0)
            {
                return compare;
            }

            return Comparer.Compare(ChildProperty, other.ChildProperty);
        }

        public override string ToString()
        {
            return $"{{ {Field}, {Property}, {ChildField}, {ChildProperty} }}";
        }

        public override int CompareTo(SampleComparableBaseObject<TMember> other)
        {
            return CompareTo(other as SampleComparableChildObject<TMember>);
        }
    }
}
