using System.Collections;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.Samples.Comparers
{
    internal sealed class SampleStructComparer<TMember> : IComparer<SampleStruct<TMember>>, IComparer
    {
        private readonly IComparer<TMember> _memberComparer;

        public SampleStructComparer(IComparer<TMember> memberComparer = null)
        {
            _memberComparer = memberComparer ?? Comparer<TMember>.Default;
        }

        public int Compare(object x, object y)
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

            return Compare((SampleStruct<TMember>)x, (SampleStruct<TMember>)y);
        }

        public int Compare(SampleStruct<TMember> x, SampleStruct<TMember> y)
        {
            var compare = _memberComparer.Compare(x.Field, y.Field);
            if (compare != 0)
            {
                return compare;
            }

            return _memberComparer.Compare(x.Property, y.Property);
        }
    }
}
