using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.Samples
{
    public sealed class SampleObject
    {
        public int KeyField;
        public string ValueProperty { get; set; }

        public static IComparer<SampleObject> SampleObjectComparer { get; } =
            new KeyFieldValuePropertyRelationalComparer();

        public override string ToString() =>
            $"{nameof(KeyField)}: {KeyField}, {nameof(ValueProperty)}: {ValueProperty}";

        private sealed class KeyFieldValuePropertyRelationalComparer : IComparer<SampleObject>
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

                var keyFieldComparison = x.KeyField.CompareTo(y.KeyField);
                if (keyFieldComparison != 0)
                {
                    return keyFieldComparison;
                }

                return string.Compare(x.ValueProperty, y.ValueProperty, StringComparison.Ordinal);
            }
        }
    }
}
