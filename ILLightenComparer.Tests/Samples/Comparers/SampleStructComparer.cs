using System.Collections.Generic;

namespace ILLightenComparer.Tests.Samples.Comparers
{
    internal sealed class SampleStructComparer<TMember> : IComparer<SampleStruct<TMember>>
    {
        private readonly IComparer<TMember> _memberComparer;

        public SampleStructComparer(IComparer<TMember> memberComparer = null)
        {
            _memberComparer = memberComparer ?? Comparer<TMember>.Default;
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
