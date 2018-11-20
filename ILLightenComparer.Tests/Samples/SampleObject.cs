using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.Samples
{
    public sealed class SampleObject
    {
        public int KeyField;
        public string ValueField;
        public int KeyProperty { get; set; }
        public string ValueProperty { get; set; }

        public override string ToString() =>
            $"{nameof(KeyField)}: {KeyField}, {nameof(KeyProperty)}: {KeyProperty}, {nameof(ValueField)}: {ValueField}, {nameof(ValueProperty)}: {ValueProperty}";

        public static IComparer<SampleObject> SampleObjectComparer { get; } = new SampleObjectRelationalComparer();

        private sealed class SampleObjectRelationalComparer : IComparer<SampleObject>
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
    }
}
