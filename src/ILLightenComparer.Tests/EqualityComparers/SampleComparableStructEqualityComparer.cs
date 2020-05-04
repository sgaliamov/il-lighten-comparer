﻿using System.Collections;
using System.Collections.Generic;
using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.EqualityComparers
{
    internal sealed class SampleComparableStructEqualityComparer<TMember> : IEqualityComparer<SampleComparableStruct<TMember>>, IEqualityComparer
    {
        private readonly IEqualityComparer _memberComparer;

        public SampleComparableStructEqualityComparer(IEqualityComparer memberComparer = null) => _memberComparer = memberComparer ?? EqualityComparer<TMember>.Default;

        public bool Equals(SampleComparableStruct<TMember> x, SampleComparableStruct<TMember> y)
        {
            var compare = _memberComparer.Equals(x.Field, y.Field);
            if (!compare) {
                return false;
            }

            return _memberComparer.Equals(x.Property, y.Property);
        }

        bool IEqualityComparer.Equals(object x, object y)
        {
            if (ReferenceEquals(x, y)) {
                return true;
            }

            if (y is null || x is null) {
                return false;
            }

            return Equals((SampleComparableStruct<TMember>)x, (SampleComparableStruct<TMember>)y);
        }

        public int GetHashCode(SampleComparableStruct<TMember> obj)
        {
            var setter = _memberComparer as IHashSeedSetter;
            var combiner = HashCodeCombiner.Start();

            setter?.SetHashSeed(combiner.CombinedHash);
            combiner.CombineObjects(obj.Field is null ? 0 : _memberComparer.GetHashCode(obj.Field));

            setter?.SetHashSeed(combiner.CombinedHash);
            return combiner.CombineObjects(obj.Property is null ? 0 : _memberComparer.GetHashCode(obj.Property));
        }

        public int GetHashCode(object obj) => GetHashCode((SampleComparableStruct<TMember>)obj);
    }
}