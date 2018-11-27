using System;
using System.Collections.Generic;
using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.ComparerTests.NullableTests
{
    public struct NullableSampleStruct
    {
        public EnumSmall? EnumField;
        public int? Field;
        public EnumSmall? EnumProperty { get; set; }
        public int? Property { get; set; }

        private sealed class NullableSampleStructRelationalComparer : IComparer<NullableSampleStruct>
        {
            public int Compare(NullableSampleStruct x, NullableSampleStruct y)
            {
                var enumFieldComparison = Nullable.Compare(x.EnumField, y.EnumField);
                if (enumFieldComparison != 0)
                {
                    return enumFieldComparison;
                }

                var fieldComparison = Nullable.Compare(x.Field, y.Field);
                if (fieldComparison != 0)
                {
                    return fieldComparison;
                }

                var enumPropertyComparison = Nullable.Compare(x.EnumProperty, y.EnumProperty);
                if (enumPropertyComparison != 0)
                {
                    return enumPropertyComparison;
                }

                return Nullable.Compare(x.Property, y.Property);
            }
        }

        public static IComparer<NullableSampleStruct> Comparer { get; } =
            new NullableSampleStructRelationalComparer();
    }
}
