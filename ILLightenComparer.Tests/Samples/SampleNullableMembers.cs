using System.Collections.Generic;

namespace ILLightenComparer.Tests.Samples
{
    public sealed class SampleNullableMembers<TMember> where TMember : struct
    {
        public TMember? Field;

        public TMember? Property { get; set; }

        public static IComparer<SampleNullableMembers<TMember>> CreateComparer(IComparer<TMember?> comparer)
        {
            return new RelationalComparer(comparer);
        }

        private sealed class RelationalComparer : IComparer<SampleNullableMembers<TMember>>
        {
            private readonly IComparer<TMember?> _memberComparer;

            public RelationalComparer(IComparer<TMember?> memberComparer)
            {
                _memberComparer = memberComparer;
            }

            public int Compare(SampleNullableMembers<TMember> x, SampleNullableMembers<TMember> y)
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
