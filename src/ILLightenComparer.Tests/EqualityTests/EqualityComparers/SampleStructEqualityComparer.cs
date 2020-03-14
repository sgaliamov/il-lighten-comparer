using System;
using System.Collections;
using System.Collections.Generic;
using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.EqualityTests.EqualityComparers
{
    internal sealed class SampleStructEqualityComparer<TMember> : IEqualityComparer<SampleStruct<TMember>>, IEqualityComparer
    {
        private readonly IEqualityComparer<TMember> _memberComparer;

        public SampleStructEqualityComparer(IEqualityComparer<TMember> memberComparer = null) => _memberComparer = memberComparer ?? EqualityComparer<TMember>.Default;

        public bool Equals(SampleStruct<TMember> x, SampleStruct<TMember> y)
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

            return Equals((SampleStruct<TMember>)x, (SampleStruct<TMember>)y);
        }

        public int GetHashCode(SampleStruct<TMember> obj) => HashCode.Combine(obj.Field, obj.Property);

        public int GetHashCode(object obj) => GetHashCode((SampleStruct<TMember>)obj);
    }
}
