using System;
using System.Collections;
using System.Collections.Generic;
using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.EqualityTests.EqualityComparers
{
    internal sealed class SampleObjectEqualityComparer<TMember> : IEqualityComparer<SampleObject<TMember>>, IEqualityComparer
    {
        private readonly IEqualityComparer<TMember> _memberComparer;

        public SampleObjectEqualityComparer(IEqualityComparer<TMember> memberComparer = null) => _memberComparer = memberComparer ?? EqualityComparer<TMember>.Default;

        public bool Equals(SampleObject<TMember> x, SampleObject<TMember> y)
        {
            if (ReferenceEquals(x, y)) {
                return true;
            }

            if (y is null || x is null) {
                return false;
            }

            var compare = _memberComparer.Equals(x.Field, y.Field);
            if (!compare) {
                return false;
            }

            return _memberComparer.Equals(x.Property, y.Property);
        }

        bool IEqualityComparer.Equals(object x, object y) => Equals((SampleObject<TMember>)x, (SampleObject<TMember>)y);

        public int GetHashCode(SampleObject<TMember> obj) => HashCode.Combine(obj.Field, obj.Property);

        public int GetHashCode(object obj) => GetHashCode((SampleObject<TMember>)obj);
    }
}
