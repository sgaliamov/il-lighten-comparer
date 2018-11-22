using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.Samples
{
    public sealed class SampleObject
    {
        public SampleEnum EnumField;
        public int KeyField;
        public bool? NullableField;
        public string ValueField;

        public static IComparer<SampleObject> SampleObjectComparer { get; } = new SampleObjectRelationalComparer();

        public SampleEnum EnumProperty { get; set; }
        public int KeyProperty { get; set; }
        public decimal? NullableProperty { get; set; }
        public string ValueProperty { get; set; }

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

                var nullableFieldComparison = Nullable.Compare(x.NullableField, y.NullableField);
                if (nullableFieldComparison != 0)
                {
                    return nullableFieldComparison;
                }

                var valueFieldComparison = string.Compare(x.ValueField, y.ValueField, StringComparison.Ordinal);
                if (valueFieldComparison != 0)
                {
                    return valueFieldComparison;
                }

                var enumPropertyComparison = x.EnumProperty.CompareTo(y.EnumProperty);
                if (enumPropertyComparison != 0)
                {
                    return enumPropertyComparison;
                }

                var keyPropertyComparison = x.KeyProperty.CompareTo(y.KeyProperty);
                if (keyPropertyComparison != 0)
                {
                    return keyPropertyComparison;
                }

                var nullablePropertyComparison = Nullable.Compare(x.NullableProperty, y.NullableProperty);
                if (nullablePropertyComparison != 0)
                {
                    return nullablePropertyComparison;
                }

                return string.Compare(x.ValueProperty, y.ValueProperty, StringComparison.Ordinal);
            }
        }
    }
}
