using System;
using System.Collections.Generic;
using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples
{
    public class BaseNestedObject : AbstractNestedObject
    {
        public new static IComparer<BaseNestedObject> Comparer { get; } = new RelationalComparer();
        public EnumSmall? Key { get; set; }

        private sealed class RelationalComparer : IComparer<BaseNestedObject>
        {
            public int Compare(BaseNestedObject x, BaseNestedObject y)
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

                var compare = Nullable.Compare(x.Key, y.Key);
                if (compare != 0)
                {
                    return compare;
                }

                return AbstractNestedObject.Comparer.Compare(x, y);
            }
        }
    }
}
