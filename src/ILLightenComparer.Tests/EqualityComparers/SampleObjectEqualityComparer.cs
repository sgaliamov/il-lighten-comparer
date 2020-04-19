using System.Collections;
using System.Collections.Generic;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Utilities;

namespace ILLightenComparer.Tests.EqualityComparers
{
    internal sealed class SampleObjectEqualityComparer<TMember> : IEqualityComparer<SampleObject<TMember>>, IEqualityComparer
    {
        private readonly IEqualityComparer _memberComparer;

        public SampleObjectEqualityComparer(IEqualityComparer memberComparer = null) => _memberComparer = memberComparer ?? EqualityComparer<TMember>.Default;

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

        public int GetHashCode(SampleObject<TMember> obj) => HashCodeCombiner
            .Start()
            .Combine(_memberComparer, obj.Field?.ObjectToArray())
            .Combine(_memberComparer, obj.Property?.ObjectToArray());

        public int GetHashCode(object obj) => GetHashCode((SampleObject<TMember>)obj);
    }
}
