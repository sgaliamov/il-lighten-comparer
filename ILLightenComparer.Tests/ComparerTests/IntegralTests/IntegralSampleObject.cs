using System.Collections.Generic;
using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.ComparerTests.IntegralTests
{
    public class IntegralSampleObject
    {
        public char CharField;
        public SmallEnum EnumField;
        public short Field;
        public char CharProperty { get; set; }
        public SmallEnum EnumProperty { get; set; }
        public short Property { get; set; }

        private sealed class IntegralSampleObjectRelationalComparer : IComparer<IntegralSampleObject>
        {
            public int Compare(IntegralSampleObject x, IntegralSampleObject y)
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

                var charFieldComparison = x.CharField.CompareTo(y.CharField);
                if (charFieldComparison != 0)
                {
                    return charFieldComparison;
                }

                var enumFieldComparison = x.EnumField.CompareTo(y.EnumField);
                if (enumFieldComparison != 0)
                {
                    return enumFieldComparison;
                }

                var fieldComparison = x.Field.CompareTo(y.Field);
                if (fieldComparison != 0)
                {
                    return fieldComparison;
                }

                var charPropertyComparison = x.CharProperty.CompareTo(y.CharProperty);
                if (charPropertyComparison != 0)
                {
                    return charPropertyComparison;
                }

                var enumPropertyComparison = x.EnumProperty.CompareTo(y.EnumProperty);
                if (enumPropertyComparison != 0)
                {
                    return enumPropertyComparison;
                }

                return x.Property.CompareTo(y.Property);
            }
        }

        public static IComparer<IntegralSampleObject> Comparer { get; } = new IntegralSampleObjectRelationalComparer();
    }
}
