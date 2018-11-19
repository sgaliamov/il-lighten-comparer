//using System;
//using System.Collections.Generic;
//using ILLightenComparer.Benchmarks.Models;

//namespace ILLightenComparer.Benchmarks.Comparers
//{
//    public sealed class FlatObjectRelationalComparer : IComparer<FlatObject>
//    {
//        public int Compare(FlatObject x, FlatObject y)
//        {
//            if (ReferenceEquals(x, y))
//            {
//                return 0;
//            }

//            if (ReferenceEquals(null, y))
//            {
//                return 1;
//            }

//            if (ReferenceEquals(null, x))
//            {
//                return -1;
//            }

//            //var booleanFieldComparison = Nullable.Compare(x.BooleanField, y.BooleanField);
//            //if (booleanFieldComparison != 0)
//            //{
//            //    return booleanFieldComparison;
//            //}

//            var integerFieldComparison = x.IntegerField.CompareTo(y.IntegerField);
//            if (integerFieldComparison != 0)
//            {
//                return integerFieldComparison;
//            }

//            //var stringFieldComparison = string.Compare(x.StringField, y.StringField, StringComparison.Ordinal);
//            //if (stringFieldComparison != 0)
//            //{
//            //    return stringFieldComparison;
//            //}

//            //var dateTimeComparison = Nullable.Compare(x.DateTime, y.DateTime);
//            //if (dateTimeComparison != 0)
//            //{
//            //    return dateTimeComparison;
//            //}

//            var doubleComparison = x.Double.CompareTo(y.Double);
//            if (doubleComparison != 0)
//            {
//                return doubleComparison;
//            }

//            var decimalComparison = x.Decimal.CompareTo(y.Decimal);
//            if (decimalComparison != 0)
//            {
//                return decimalComparison;
//            }

//            //var floatComparison = Nullable.Compare(x.Float, y.Float);
//            //if (floatComparison != 0)
//            //{
//            //    return floatComparison;
//            //}

//            return x.Byte.CompareTo(y.Byte);

//            //return string.Compare(x.StringProperty, y.StringProperty, StringComparison.Ordinal);
//        }
//    }
//}
