using System;
using System.Collections;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.Samples
{
    public struct SampleStruct
    {
        public int Key { get; set; }
        public string Value { get; set; }

        private sealed class SampleStructRelationalComparer :
            IComparer<SampleStruct>,
            IComparer
        {
            public int Compare(object x, object y) => Compare((SampleStruct)x, (SampleStruct)y);

            public int Compare(SampleStruct x, SampleStruct y)
            {
                var keyComparison = x.Key.CompareTo(y.Key);
                if (keyComparison != 0)
                {
                    return keyComparison;
                }

                return string.Compare(x.Value, y.Value, StringComparison.Ordinal);
            }
        }

        public static IComparer<SampleStruct> Comparer { get; } = new SampleStructRelationalComparer();
    }
}
