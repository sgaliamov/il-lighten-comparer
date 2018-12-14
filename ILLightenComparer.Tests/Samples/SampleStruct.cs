using System.Collections.Generic;

namespace ILLightenComparer.Tests.Samples
{
    public struct SampleStruct<TMember>
    {
        public TMember Field;

        public TMember Property { get; set; }

        public static IComparer<SampleStruct<TMember>> CreateComparer(IComparer<TMember> comparer)
        {
            return new RelationalComparer(comparer);
        }

        private sealed class RelationalComparer : IComparer<SampleStruct<TMember>>
        {
            private readonly IComparer<TMember> _memberComparer;

            public RelationalComparer(IComparer<TMember> memberComparer)
            {
                _memberComparer = memberComparer;
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
}
