using System;
using System.Collections;
using System.Collections.Generic;
using ILLightenComparer.Benchmarks.Models;

namespace ILLightenComparer.Benchmarks.Comparers
{
    public sealed class FlatObjectRelationalComparer : IComparer<FlatObject>
    {
        public int Compare(FlatObject x, FlatObject y)
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

            var dateTimeComparison = Nullable.Compare(x.DateTime, y.DateTime);
            if (dateTimeComparison != 0)
            {
                return dateTimeComparison;
            }

            var doubleComparison = x.Double.CompareTo(y.Double);
            if (doubleComparison != 0)
            {
                return doubleComparison;
            }

            var booleanComparison = x.Boolean.CompareTo(y.Boolean);
            if (booleanComparison != 0)
            {
                return booleanComparison;
            }

            var objectComparison = Comparer.Default.Compare(x.Object, y.Object);
            if (objectComparison != 0)
            {
                return objectComparison;
            }

            var floatComparison = Nullable.Compare(x.Float, y.Float);
            if (floatComparison != 0)
            {
                return floatComparison;
            }

            var byteComparison = x.Byte.CompareTo(y.Byte);
            if (byteComparison != 0)
            {
                return byteComparison;
            }

            var integerComparison = x.Integer.CompareTo(y.Integer);
            if (integerComparison != 0)
            {
                return integerComparison;
            }

            return string.Compare(x.String, y.String, StringComparison.Ordinal);
        }
    }
}
