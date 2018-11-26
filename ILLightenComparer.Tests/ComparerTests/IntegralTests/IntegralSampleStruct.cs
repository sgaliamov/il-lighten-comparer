using System.Collections.Generic;
using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.ComparerTests.IntegralTests
{
    public struct IntegralSampleStruct
    {
        public SmallEnum EnumField;
        public int Field;

        public SmallEnum EnumProperty { get; set; }
        public int Property { get; set; }

        public override string ToString() =>
            $"{nameof(EnumField)}: {EnumField}, {nameof(Field)}: {Field}, {nameof(EnumProperty)}: {EnumProperty}, {nameof(Property)}: {Property}";

        private sealed class IntegralSampleStructRelationalComparer : IComparer<IntegralSampleStruct>
        {
            public int Compare(IntegralSampleStruct x, IntegralSampleStruct y)
            {
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

        public static IComparer<IntegralSampleStruct> Comparer { get; } =
            new IntegralSampleStructRelationalComparer();
    }
}
