using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.ComparableTests
{
    public struct ComparableSampleStruct
    {
        public int Property { get; set; }
        public int Field;

        public override string ToString() => $"{nameof(Field)}: {Field}, {nameof(Property)}: {Property}";

        private sealed class FieldPropertyRelationalComparer : IComparer<ComparableSampleStruct>
        {
            public int Compare(ComparableSampleStruct x, ComparableSampleStruct y)
            {
                var fieldComparison = x.Field.CompareTo(y.Field);
                if (fieldComparison != 0)
                {
                    return fieldComparison;
                }

                return x.Property.CompareTo(y.Property);
            }
        }

        public static IComparer<ComparableSampleStruct> Comparer { get; } =
            new FieldPropertyRelationalComparer();
    }
}
