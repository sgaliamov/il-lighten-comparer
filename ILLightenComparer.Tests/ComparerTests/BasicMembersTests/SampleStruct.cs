using System.Collections.Generic;
using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.ComparerTests.BasicMembersTests
{
    public struct SampleStruct
    {
        public EnumBig EnumField;
        public int Field;
        public EnumBig EnumProperty { get; set; }
        public int Property { get; set; }

        public override string ToString() =>
            $"{nameof(EnumField)}: {EnumField}, {nameof(Field)}: {Field}, {nameof(EnumProperty)}: {EnumProperty}, {nameof(Property)}: {Property}";

        private sealed class ComparableSampleStructRelationalComparer : IComparer<SampleStruct>
        {
            public int Compare(SampleStruct x, SampleStruct y)
            {
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

                var enumPropertyComparison = x.EnumProperty.CompareTo(y.EnumProperty);
                if (enumPropertyComparison != 0)
                {
                    return enumPropertyComparison;
                }

                return x.Property.CompareTo(y.Property);
            }
        }

        public static IComparer<SampleStruct> Comparer { get; } =
            new ComparableSampleStructRelationalComparer();
    }
}
