using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples.Nested
{
    public struct NestedStruct
    {
        public ulong Value { get; set; }
        public ulong? NullableValue { get; set; }

        public static RelationalComparer Comparer { get; } = new RelationalComparer();

        public sealed class RelationalComparer : IComparer<NestedStruct>
        {
            public int Compare(NestedStruct x, NestedStruct y)
            {
                var valueComparison = x.Value.CompareTo(y.Value);
                if (valueComparison != 0)
                {
                    return valueComparison;
                }

                return Nullable.Compare(x.NullableValue, y.NullableValue);
            }

            public int Compare(NestedStruct? x, NestedStruct? y)
            {
                if (x.HasValue)
                {
                    if (y.HasValue)
                    {
                        return Compare(x.Value, y.Value);
                    }

                    return 1;
                }

                if (y.HasValue)
                {
                    return -1;
                }

                return 0;
            }
        }
    }
}
