using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.Samples
{
    public sealed class SampleObject
    {
        public int Key { get; set; }
        public string Value { get; set; }

        public static IComparer<SampleObject> Comparer { get; } = new KeyValueRelationalComparer();

        public override string ToString() => $"{nameof(Key)}: {Key}, {nameof(Value)}: {Value}";

        private sealed class KeyValueRelationalComparer : IComparer<SampleObject>
        {
            public int Compare(SampleObject x, SampleObject y)
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

                var keyComparison = x.Key.CompareTo(y.Key);
                if (keyComparison != 0)
                {
                    return keyComparison;
                }

                return string.Compare(x.Value, y.Value, StringComparison.Ordinal);
            }
        }
    }
}
