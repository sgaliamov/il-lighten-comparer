using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.Samples
{
    public struct SampleStruct
    {
        public int KeyField;
        public string ValueProperty { get; set; }

        public override string ToString() =>
            $"{nameof(ValueProperty)}: {ValueProperty}, {nameof(KeyField)}: {KeyField}";

        private sealed class KeyFieldValuePropertyRelationalComparer : IComparer<SampleStruct>
        {
            public int Compare(SampleStruct x, SampleStruct y)
            {
                var keyFieldComparison = x.KeyField.CompareTo(y.KeyField);
                if (keyFieldComparison != 0)
                {
                    return keyFieldComparison;
                }

                return string.Compare(x.ValueProperty, y.ValueProperty, StringComparison.Ordinal);
            }
        }

        public static IComparer<SampleStruct> SampleStructComparer { get; } =
            new KeyFieldValuePropertyRelationalComparer();
    }
}
