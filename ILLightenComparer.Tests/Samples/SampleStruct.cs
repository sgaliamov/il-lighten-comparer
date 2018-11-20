using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.Samples
{
    public struct SampleStruct
    {
        public int KeyField;
        public string ValueField;
        public int KeyProperty { get; set; }
        public string ValueProperty { get; set; }

        private sealed class SampleStructRelationalComparer : IComparer<SampleStruct>
        {
            public int Compare(SampleStruct x, SampleStruct y)
            {
                var keyFieldComparison = x.KeyField.CompareTo(y.KeyField);
                if (keyFieldComparison != 0)
                {
                    return keyFieldComparison;
                }

                var keyPropertyComparison = x.KeyProperty.CompareTo(y.KeyProperty);
                if (keyPropertyComparison != 0)
                {
                    return keyPropertyComparison;
                }

                var valueFieldComparison = string.Compare(x.ValueField, y.ValueField, StringComparison.Ordinal);
                if (valueFieldComparison != 0)
                {
                    return valueFieldComparison;
                }

                return string.Compare(x.ValueProperty, y.ValueProperty, StringComparison.Ordinal);
            }
        }

        public override string ToString() =>
            $"{nameof(KeyField)}: {KeyField}, {nameof(KeyProperty)}: {KeyProperty}, {nameof(ValueField)}: {ValueField}, {nameof(ValueProperty)}: {ValueProperty}";

        public static IComparer<SampleStruct> SampleStructComparer { get; } = new SampleStructRelationalComparer();
    }
}
