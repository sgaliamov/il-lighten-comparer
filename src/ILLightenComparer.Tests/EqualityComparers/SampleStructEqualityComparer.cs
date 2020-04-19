using System.Collections;
using System.Collections.Generic;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Utilities;

namespace ILLightenComparer.Tests.EqualityComparers
{
    internal sealed class SampleStructEqualityComparer<TMember> : IEqualityComparer<SampleStruct<TMember>>, IEqualityComparer
    {
        private readonly IEqualityComparer _memberComparer;

        public SampleStructEqualityComparer(IEqualityComparer memberComparer = null) => _memberComparer = memberComparer ?? EqualityComparer<TMember>.Default;

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

        public int GetHashCode(SampleStruct<TMember> obj) => HashCodeCombiner
            .Start()
            .Combine(_memberComparer, obj.Field?.ObjectToArray())
            .Combine(_memberComparer, obj.Property?.ObjectToArray());

        public int GetHashCode(object obj) => GetHashCode((SampleStruct<TMember>)obj);
    }
}
