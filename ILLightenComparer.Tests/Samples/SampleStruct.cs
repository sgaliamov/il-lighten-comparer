using System.Collections;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.Samples
{
    public struct SampleStruct
    {
        public int Key { get; set; }
        public decimal Value { get; set; } // todo: change type to string

        private sealed class SampleStructRelationalComparer :
            IComparer<SampleStruct>,
            IComparer
        {
            public int Compare(object x, object y) => CompareS((SampleStruct)x, (SampleStruct)y);

            public int Compare(SampleStruct x, SampleStruct y) => CompareS(x, y);

            private static int CompareS(SampleStruct x, SampleStruct y)
            {
                var keyComparison = x.Key.CompareTo(y.Key);
                if (keyComparison != 0)
                {
                    return keyComparison;
                }

                var valueComparison = x.Value.CompareTo(y.Value);
                if (valueComparison != 0)
                {
                    return valueComparison;
                }

                return valueComparison;
            }
        }

        public override string ToString() => $"{nameof(Key)}: {Key}, {nameof(Value)}: {Value}";

        public static IComparer Comparer { get; } = new SampleStructRelationalComparer();
    }
}
