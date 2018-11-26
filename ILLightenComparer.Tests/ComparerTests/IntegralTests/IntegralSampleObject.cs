using System.Collections.Generic;
using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.ComparerTests.IntegralTests
{
    public class IntegralSampleObject
    {
        public SmallEnum EnumField;
        public short Field;
        public SmallEnum EnumProperty { get; set; }
        public short Property { get; set; }

        public static IComparer<IntegralSampleObject> Comparer { get; } =
            new IntegralSampleObjectRelationalComparer();

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

                var enumFieldComparison = x.EnumField - y.EnumField;
                if (enumFieldComparison != 0)
                {
                    return enumFieldComparison;
                }

                var fieldComparison = x.Field - y.Field;
                if (fieldComparison != 0)
                {
                    return fieldComparison;
                }

                var enumPropertyComparison = x.EnumProperty - y.EnumProperty;
                if (enumPropertyComparison != 0)
                {
                    return enumPropertyComparison;
                }

                return x.Property - y.Property;
            }
        }
    }
}
