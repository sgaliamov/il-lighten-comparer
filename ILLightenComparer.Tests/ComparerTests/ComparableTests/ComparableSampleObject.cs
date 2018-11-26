using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.ComparableTests
{
    public class ComparableSampleObject
    {
        public int Field;
        public int Property { get; set; }

        public static IComparer<ComparableSampleObject> Comparer { get; } =
            new FieldPropertyRelationalComparer();

        public override string ToString() => $"{nameof(Field)}: {Field}, {nameof(Property)}: {Property}";

        private sealed class FieldPropertyRelationalComparer : IComparer<ComparableSampleObject>
        {
            public int Compare(ComparableSampleObject x, ComparableSampleObject y)
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

                var fieldComparison = x.Field.CompareTo(y.Field);
                if (fieldComparison != 0)
                {
                    return fieldComparison;
                }

                return x.Property.CompareTo(y.Property);
            }
        }
    }
}
