using System.Collections;
using System.Collections.Generic;
using ILLightenComparer.Tests.Samples;

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

        public int GetHashCode(SampleObject<TMember> obj)
        {
            var setter = _memberComparer as IHashSeedSetter;
            var combiner = HashCodeCombiner.Start();

            setter?.Set(combiner.CombinedHash);
            var fieldHash = _memberComparer.GetHashCode(obj.Field);

            combiner.CombineObjects(fieldHash);

            setter?.Set(combiner.CombinedHash);
            var propertyHash = _memberComparer.GetHashCode(obj.Property);

            return combiner.CombineObjects(propertyHash);
        }

        public int GetHashCode(object obj) => GetHashCode((SampleObject<TMember>)obj);
    }
}
