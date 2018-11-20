using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.Samples
{
    public struct SampleStruct
    {
        public int KeyProperty { get; set; }
        public string ValueProperty { get; set; }

        public int KeyField;
        public string ValueField;

        public override string ToString() =>
            $"{nameof(KeyField)}: {KeyField}, {nameof(ValueField)}: {ValueField}, {nameof(KeyProperty)}: {KeyProperty}, {nameof(ValueProperty)}: {ValueProperty}";

        private sealed class SampleStructRelationalComparer : IComparer<SampleStruct>
        {
            public int Compare(SampleStruct x, SampleStruct y)
            {
                var keyFieldComparison = x.KeyField.CompareTo(y.KeyField);
                if (keyFieldComparison != 0)
                {
                    return keyFieldComparison;
                }

                var valueFieldComparison = string.Compare(x.ValueField, y.ValueField, StringComparison.Ordinal);
                if (valueFieldComparison != 0)
                {
                    return valueFieldComparison;
                }

                var keyComparison = x.KeyProperty.CompareTo(y.KeyProperty);
                if (keyComparison != 0)
                {
                    return keyComparison;
                }

                return string.Compare(x.ValueProperty, y.ValueProperty, StringComparison.Ordinal);
            }
        }

        public static IComparer<SampleStruct> SampleStructComparer { get; } = new SampleStructRelationalComparer();
    }
}
