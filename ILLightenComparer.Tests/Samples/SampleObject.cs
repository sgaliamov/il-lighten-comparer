using System.Collections.Generic;

namespace ILLightenComparer.Tests.Samples
{
    public sealed class SampleObject<TMember>
    {
        public TMember Field;

        public TMember Property { get; set; }

        public static IComparer<SampleObject<TMember>> CreateComparer(IComparer<TMember> comparer = null)
        {
            return new RelationalComparer(comparer ?? Comparer<TMember>.Default);
        }

        private sealed class RelationalComparer : IComparer<SampleObject<TMember>>
        {
            private readonly IComparer<TMember> _memberComparer;

            public RelationalComparer(IComparer<TMember> memberComparer)
            {
                _memberComparer = memberComparer;
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
}
