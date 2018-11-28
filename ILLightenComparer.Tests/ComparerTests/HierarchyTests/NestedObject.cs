using System;
using System.Collections.Generic;
using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests
{
    public sealed class NestedObject
    {
        public static IComparer<NestedObject> Comparer { get; } = new NestedObjectComparer();

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

                var keyComparison = Nullable.Compare(x.Key, y.Key);
                if (keyComparison != 0)
                {
                    return keyComparison;
                }

                return string.Compare(x.Text, y.Text, StringComparison.Ordinal);
            }
        }
    }
}
