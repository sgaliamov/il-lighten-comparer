using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.StringTests
{
    public struct StringSampleStruct
    {
        public string Property { get; set; }
        public string Field;

        public override string ToString()
        {
            return $"{nameof(Field)}: {Field}, {nameof(Property)}: {Property}";
        }

        private sealed class FieldPropertyRelationalComparer : IComparer<StringSampleStruct>
        {
            public int Compare(StringSampleStruct x, StringSampleStruct y)
            {
                var fieldComparison = string.Compare(x.Field, y.Field, StringComparison.Ordinal);
                if (fieldComparison != 0)
                {
                    return fieldComparison;
                }

                return string.Compare(x.Property, y.Property, StringComparison.Ordinal);
            }
        }

        public static IComparer<StringSampleStruct> Comparer { get; } = new FieldPropertyRelationalComparer();
    }
}
