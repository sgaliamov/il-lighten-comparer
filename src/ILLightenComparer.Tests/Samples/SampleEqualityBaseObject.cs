﻿using System;
using System.Collections.Generic;
using ILLightenComparer.Tests.EqualityComparers;
using ILLightenComparer.Tests.Utilities;

namespace ILLightenComparer.Tests.Samples
{
    public class SampleEqualityBaseObject<TMember> : IComparable<SampleEqualityBaseObject<TMember>>
    {
        private static readonly IComparer<TMember> Comparer = Comparer<TMember>.Default;

        public TMember Field;
        public TMember Property { get; set; }

        public override string ToString() => this.ToJson();

        public override bool Equals(object obj) => Equals((SampleEqualityBaseObject<TMember>)obj);

        public bool Equals(SampleEqualityBaseObject<TMember> other) =>
            other != null
            && EqualityComparer<TMember>.Default.Equals(Field, other.Field)
            && EqualityComparer<TMember>.Default.Equals(Property, other.Property);

        public override int GetHashCode() => HashCodeCombiner.Combine(Field, Property);

        public virtual int CompareTo(SampleEqualityBaseObject<TMember> other)
        {
            if (ReferenceEquals(this, other)) {
                return 0;
            }

            if (other is null) {
                return 1;
            }

            var compare = Comparer.Compare(Field, other.Field);
            if (compare != 0) {
                return compare;
            }

            return Comparer.Compare(Property, other.Property);
        }
    }
}