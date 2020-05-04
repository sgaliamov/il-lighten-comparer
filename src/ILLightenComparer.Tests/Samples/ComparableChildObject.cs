﻿using System;
using System.Collections.Generic;
using ILLightenComparer.Tests.Utilities;

namespace ILLightenComparer.Tests.Samples
{
    public sealed class ComparableChildObject<TMember> :
        ComparableBaseObject<TMember>,
        IComparable<ComparableChildObject<TMember>>
    {
        public static IComparer<TMember> ChildComparer = Comparer<TMember>.Default;

        public TMember ChildField;
        public TMember ChildProperty { get; set; }

        public int CompareTo(ComparableChildObject<TMember> other)
        {
            if (ReferenceEquals(this, other)) {
                return 0;
            }

            if (other is null) {
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

        public override string ToString() => this.ToJson();

        public override int CompareTo(ComparableBaseObject<TMember> other) => CompareTo(other as ComparableChildObject<TMember>);
    }
}