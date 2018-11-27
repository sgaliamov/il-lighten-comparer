using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.NullableTests
{
    public class NullableSampleObject
    {
        //public BigEnum? EnumField;
        //public int? Field;
        public short? EnumProperty { get; set; }
        public int? Property { get; set; }

        public static IComparer<NullableSampleObject> Comparer { get; } =
            new NullableSampleObjectRelationalComparer();

        private sealed class NullableSampleObjectRelationalComparer : IComparer<NullableSampleObject>
        {
            public int Compare(NullableSampleObject x, NullableSampleObject y)
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

                //var enumFieldComparison = Nullable.Compare(x.EnumField, y.EnumField);
                //if (enumFieldComparison != 0)
                //{
                //    return enumFieldComparison;
                //}

                //var fieldComparison = Nullable.Compare(x.Field, y.Field);
                //if (fieldComparison != 0)
                //{
                //    return fieldComparison;
                //}

                var enumPropertyComparison = Nullable.Compare(x.EnumProperty, y.EnumProperty);
                if (enumPropertyComparison != 0)
                {
                    return enumPropertyComparison;
                }

                return Nullable.Compare(x.Property, y.Property);
            }
        }
    }
}
