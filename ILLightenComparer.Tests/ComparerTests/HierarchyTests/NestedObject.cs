using System;
using System.Collections.Generic;
using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests
{
    public sealed class NestedObject
    {
        public DeepNestedObject DeepNestedField;
        public static IComparer<NestedObject> Comparer { get; } = new NestedObjectComparer();
        public DeepNestedObject DeepNestedProperty { get; set; }
        public EnumSmall? Key { get; set; }
        public string Text { get; set; }

        private sealed class NestedObjectComparer : IComparer<NestedObject>
        {
            public int Compare(NestedObject x, NestedObject y)
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

                var compare = DeepNestedObject.Comparer.Compare(x.DeepNestedField, y.DeepNestedField);
                if (compare != 0)
                {
                    return compare;
                }

                compare = Nullable.Compare(x.Key, y.Key);
                if (compare != 0)
                {
                    return compare;
                }

                compare = DeepNestedObject.Comparer.Compare(x.DeepNestedProperty, y.DeepNestedProperty);
                if (compare != 0)
                {
                    return compare;
                }

                return string.Compare(x.Text, y.Text, StringComparison.Ordinal);
            }
        }
    }
}
