using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.Samples
{
    public struct SampleStruct
    {
        public SampleEnum EnumField;
        public int KeyField;
        public string ValueField;
        public int KeyProperty { get; set; }
        public string ValueProperty { get; set; }

        public override string ToString() =>
            $"{nameof(EnumField)}: {EnumField}, {nameof(KeyField)}: {KeyField}, {nameof(KeyProperty)}: {KeyProperty}, {nameof(ValueField)}: {ValueField}, {nameof(ValueProperty)}: {ValueProperty}";

        private sealed class SampleStructRelationalComparer : IComparer<SampleStruct>
        {
            public int Compare(SampleStruct x, SampleStruct y)
            {
                var enumFieldComparison = x.EnumField.CompareTo(y.EnumField);
                if (enumFieldComparison != 0)
                {
                    return enumFieldComparison;
                }

                var keyFieldComparison = x.KeyField.CompareTo(y.KeyField);
                if (keyFieldComparison != 0)
                {
                    return keyFieldComparison;
                }

                var valueFieldComparison = string.Compare(x.ValueField, y.ValueField, StringComparison.CurrentCulture);
                if (valueFieldComparison != 0)
                {
                    return valueFieldComparison;
                }

                var keyPropertyComparison = x.KeyProperty.CompareTo(y.KeyProperty);
                if (keyPropertyComparison != 0)
                {
                    return keyPropertyComparison;
                }

                return string.Compare(x.ValueProperty, y.ValueProperty, StringComparison.CurrentCulture);
            }
        }

        public static IComparer<SampleStruct> SampleStructComparer { get; } = new SampleStructRelationalComparer();
    }
}
