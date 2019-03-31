using System.Collections;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.Samples.Comparers
{
    internal sealed class SampleObjectComparer<TMember> : IComparer<SampleObject<TMember>>, IComparer
    {
        private readonly IComparer<TMember> _memberComparer;

        public SampleObjectComparer(IComparer<TMember> memberComparer = null)
        {
            _memberComparer = memberComparer ?? Comparer<TMember>.Default;
        }

        public int Compare(object x, object y)
        {
            return Compare(x as SampleObject<TMember>, y as SampleObject<TMember>);
        }

        public int Compare(SampleObject<TMember> x, SampleObject<TMember> y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }

            if (ReferenceEquals(null, y))
            {
                return 1;
            }

            if (ReferenceEquals(null, x))
            {
                return -1;
            }

            var compare = _memberComparer.Compare(x.Field, y.Field);
            if (compare != 0)
            {
                return compare;
            }

            return _memberComparer.Compare(x.Property, y.Property);
        }
    }
}
