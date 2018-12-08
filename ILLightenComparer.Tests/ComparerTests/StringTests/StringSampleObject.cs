using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.StringTests
{
    public class StringSampleObject
    {
        public string Field;

        public static IComparer<StringSampleObject> Comparer { get; } =
            new FieldPropertyRelationalComparer();

        public string Property { get; set; }

        public override string ToString() => $"{nameof(Field)}: {Field}, {nameof(Property)}: {Property}";

        private sealed class FieldPropertyRelationalComparer : IComparer<StringSampleObject>
        {
            public int Compare(StringSampleObject x, StringSampleObject y)
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

                var fieldComparison = string.Compare(x.Field, y.Field, StringComparison.Ordinal);
                if (fieldComparison != 0)
                {
                    return fieldComparison;
                }

                return string.Compare(x.Property, y.Property, StringComparison.Ordinal);
            }
        }
    }
}
