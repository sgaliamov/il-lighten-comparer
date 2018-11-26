using System.Collections.Generic;
using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.ComparerTests.ComparableTests
{
    public struct ComparableSampleStruct
    {
        public BigEnum EnumField;
        public int Field;
        public BigEnum EnumProperty { get; set; }
        public int Property { get; set; }

        public override string ToString() =>
            $"{nameof(EnumField)}: {EnumField}, {nameof(Field)}: {Field}, {nameof(EnumProperty)}: {EnumProperty}, {nameof(Property)}: {Property}";

        private sealed class ComparableSampleStructRelationalComparer : IComparer<ComparableSampleStruct>
        {
            public int Compare(ComparableSampleStruct x, ComparableSampleStruct y)
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

        public static IComparer<ComparableSampleStruct> Comparer { get; } =
            new ComparableSampleStructRelationalComparer();
    }
}
