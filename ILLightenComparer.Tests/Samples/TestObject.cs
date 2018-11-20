using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.Samples
{
    public class TestObject
    {
        public bool BooleanProperty { get; set; }
        public byte ByteProperty { get; set; }
        public sbyte SByteProperty { get; set; }
        public char CharProperty { get; set; }
        public decimal DecimalProperty { get; set; }
        public double DoubleProperty { get; set; }
        public float SingleProperty { get; set; }
        public int Int32Property { get; set; }
        public uint UInt32Property { get; set; }
        public long Int64Property { get; set; }
        public ulong UInt64Property { get; set; }
        public short Int16Property { get; set; }
        public ushort UInt16Property { get; set; }
        public string StringProperty { get; set; }

        public bool BooleanField;
        public byte ByteField;
        public sbyte SByteField;
        public char CharField;
        public decimal DecimalField;
        public double DoubleField;
        public float SingleField;
        public int Int32Field;
        public uint UInt32Field;
        public long Int64Field;
        public ulong UInt64Field;
        public short Int16Field;
        public ushort UInt16Field;
        public string StringField;

        private sealed class TestObjectRelationalComparer : IComparer<TestObject>
        {
            public int Compare(TestObject x, TestObject y)
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

                var booleanFieldComparison = x.BooleanField.CompareTo(y.BooleanField);
                if (booleanFieldComparison != 0)
                {
                    return booleanFieldComparison;
                }

                var byteFieldComparison = x.ByteField.CompareTo(y.ByteField);
                if (byteFieldComparison != 0)
                {
                    return byteFieldComparison;
                }

                var sByteFieldComparison = x.SByteField.CompareTo(y.SByteField);
                if (sByteFieldComparison != 0)
                {
                    return sByteFieldComparison;
                }

                var charFieldComparison = x.CharField.CompareTo(y.CharField);
                if (charFieldComparison != 0)
                {
                    return charFieldComparison;
                }

                var decimalFieldComparison = x.DecimalField.CompareTo(y.DecimalField);
                if (decimalFieldComparison != 0)
                {
                    return decimalFieldComparison;
                }

                var doubleFieldComparison = x.DoubleField.CompareTo(y.DoubleField);
                if (doubleFieldComparison != 0)
                {
                    return doubleFieldComparison;
                }

                var singleFieldComparison = x.SingleField.CompareTo(y.SingleField);
                if (singleFieldComparison != 0)
                {
                    return singleFieldComparison;
                }

                var int32FieldComparison = x.Int32Field.CompareTo(y.Int32Field);
                if (int32FieldComparison != 0)
                {
                    return int32FieldComparison;
                }

                var uInt32FieldComparison = x.UInt32Field.CompareTo(y.UInt32Field);
                if (uInt32FieldComparison != 0)
                {
                    return uInt32FieldComparison;
                }

                var int64FieldComparison = x.Int64Field.CompareTo(y.Int64Field);
                if (int64FieldComparison != 0)
                {
                    return int64FieldComparison;
                }

                var uInt64FieldComparison = x.UInt64Field.CompareTo(y.UInt64Field);
                if (uInt64FieldComparison != 0)
                {
                    return uInt64FieldComparison;
                }

                var int16FieldComparison = x.Int16Field.CompareTo(y.Int16Field);
                if (int16FieldComparison != 0)
                {
                    return int16FieldComparison;
                }

                var uInt16FieldComparison = x.UInt16Field.CompareTo(y.UInt16Field);
                if (uInt16FieldComparison != 0)
                {
                    return uInt16FieldComparison;
                }

                var stringFieldComparison = string.Compare(x.StringField, y.StringField, StringComparison.CurrentCulture);
                if (stringFieldComparison != 0)
                {
                    return stringFieldComparison;
                }

                var booleanPropertyComparison = x.BooleanProperty.CompareTo(y.BooleanProperty);
                if (booleanPropertyComparison != 0)
                {
                    return booleanPropertyComparison;
                }

                var bytePropertyComparison = x.ByteProperty.CompareTo(y.ByteProperty);
                if (bytePropertyComparison != 0)
                {
                    return bytePropertyComparison;
                }

                var sBytePropertyComparison = x.SByteProperty.CompareTo(y.SByteProperty);
                if (sBytePropertyComparison != 0)
                {
                    return sBytePropertyComparison;
                }

                var charPropertyComparison = x.CharProperty.CompareTo(y.CharProperty);
                if (charPropertyComparison != 0)
                {
                    return charPropertyComparison;
                }

                var decimalPropertyComparison = x.DecimalProperty.CompareTo(y.DecimalProperty);
                if (decimalPropertyComparison != 0)
                {
                    return decimalPropertyComparison;
                }

                var doublePropertyComparison = x.DoubleProperty.CompareTo(y.DoubleProperty);
                if (doublePropertyComparison != 0)
                {
                    return doublePropertyComparison;
                }

                var singlePropertyComparison = x.SingleProperty.CompareTo(y.SingleProperty);
                if (singlePropertyComparison != 0)
                {
                    return singlePropertyComparison;
                }

                var int32PropertyComparison = x.Int32Property.CompareTo(y.Int32Property);
                if (int32PropertyComparison != 0)
                {
                    return int32PropertyComparison;
                }

                var uInt32PropertyComparison = x.UInt32Property.CompareTo(y.UInt32Property);
                if (uInt32PropertyComparison != 0)
                {
                    return uInt32PropertyComparison;
                }

                var int64PropertyComparison = x.Int64Property.CompareTo(y.Int64Property);
                if (int64PropertyComparison != 0)
                {
                    return int64PropertyComparison;
                }

                var uInt64PropertyComparison = x.UInt64Property.CompareTo(y.UInt64Property);
                if (uInt64PropertyComparison != 0)
                {
                    return uInt64PropertyComparison;
                }

                var int16PropertyComparison = x.Int16Property.CompareTo(y.Int16Property);
                if (int16PropertyComparison != 0)
                {
                    return int16PropertyComparison;
                }

                var uInt16PropertyComparison = x.UInt16Property.CompareTo(y.UInt16Property);
                if (uInt16PropertyComparison != 0)
                {
                    return uInt16PropertyComparison;
                }

                return string.Compare(x.StringProperty, y.StringProperty, StringComparison.CurrentCulture);
            }
        }

        public static IComparer<TestObject> TestObjectComparer { get; } = new TestObjectRelationalComparer();
    }
}
