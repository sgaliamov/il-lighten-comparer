using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.Samples
{
    public class TestObject
    {
        public bool BooleanField;
        public byte ByteField;
        public char CharField;
        public decimal DecimalField;
        public double DoubleField;
        public BigEnum EnumField;
        public short Int16Field;
        public int Int32Field;
        public long Int64Field;
        public sbyte SByteField;
        public float SingleField;
        public string StringField;
        public ushort UInt16Field;
        public uint UInt32Field;
        public ulong UInt64Field;

        public static IComparer<TestObject> TestObjectComparer { get; } = new TestObjectRelationalComparer();

        public bool BooleanProperty { get; set; }
        public byte ByteProperty { get; set; }
        public char CharProperty { get; set; }
        public decimal DecimalProperty { get; set; }
        public double DoubleProperty { get; set; }
        public BigEnum EnumProperty { get; set; }
        public short Int16Property { get; set; }
        public int Int32Property { get; set; }
        public long Int64Property { get; set; }
        public sbyte SByteProperty { get; set; }
        public float SingleProperty { get; set; }
        public string StringProperty { get; set; }
        public ushort UInt16Property { get; set; }
        public uint UInt32Property { get; set; }
        public ulong UInt64Property { get; set; }

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

                var enumFieldComparison = x.EnumField.CompareTo(y.EnumField);
                if (enumFieldComparison != 0)
                {
                    return enumFieldComparison;
                }

                var int16FieldComparison = x.Int16Field.CompareTo(y.Int16Field);
                if (int16FieldComparison != 0)
                {
                    return int16FieldComparison;
                }

                var int32FieldComparison = x.Int32Field.CompareTo(y.Int32Field);
                if (int32FieldComparison != 0)
                {
                    return int32FieldComparison;
                }

                var int64FieldComparison = x.Int64Field.CompareTo(y.Int64Field);
                if (int64FieldComparison != 0)
                {
                    return int64FieldComparison;
                }

                var sByteFieldComparison = x.SByteField.CompareTo(y.SByteField);
                if (sByteFieldComparison != 0)
                {
                    return sByteFieldComparison;
                }

                var singleFieldComparison = x.SingleField.CompareTo(y.SingleField);
                if (singleFieldComparison != 0)
                {
                    return singleFieldComparison;
                }

                var stringFieldComparison =
                    string.Compare(x.StringField, y.StringField, StringComparison.InvariantCulture);
                if (stringFieldComparison != 0)
                {
                    return stringFieldComparison;
                }

                var uInt16FieldComparison = x.UInt16Field.CompareTo(y.UInt16Field);
                if (uInt16FieldComparison != 0)
                {
                    return uInt16FieldComparison;
                }

                var uInt32FieldComparison = x.UInt32Field.CompareTo(y.UInt32Field);
                if (uInt32FieldComparison != 0)
                {
                    return uInt32FieldComparison;
                }

                var uInt64FieldComparison = x.UInt64Field.CompareTo(y.UInt64Field);
                if (uInt64FieldComparison != 0)
                {
                    return uInt64FieldComparison;
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

                var enumPropertyComparison = x.EnumProperty.CompareTo(y.EnumProperty);
                if (enumPropertyComparison != 0)
                {
                    return enumPropertyComparison;
                }

                var int16PropertyComparison = x.Int16Property.CompareTo(y.Int16Property);
                if (int16PropertyComparison != 0)
                {
                    return int16PropertyComparison;
                }

                var int32PropertyComparison = x.Int32Property.CompareTo(y.Int32Property);
                if (int32PropertyComparison != 0)
                {
                    return int32PropertyComparison;
                }

                var int64PropertyComparison = x.Int64Property.CompareTo(y.Int64Property);
                if (int64PropertyComparison != 0)
                {
                    return int64PropertyComparison;
                }

                var sBytePropertyComparison = x.SByteProperty.CompareTo(y.SByteProperty);
                if (sBytePropertyComparison != 0)
                {
                    return sBytePropertyComparison;
                }

                var singlePropertyComparison = x.SingleProperty.CompareTo(y.SingleProperty);
                if (singlePropertyComparison != 0)
                {
                    return singlePropertyComparison;
                }

                var stringPropertyComparison =
                    string.Compare(x.StringProperty, y.StringProperty, StringComparison.InvariantCulture);
                if (stringPropertyComparison != 0)
                {
                    return stringPropertyComparison;
                }

                var uInt16PropertyComparison = x.UInt16Property.CompareTo(y.UInt16Property);
                if (uInt16PropertyComparison != 0)
                {
                    return uInt16PropertyComparison;
                }

                var uInt32PropertyComparison = x.UInt32Property.CompareTo(y.UInt32Property);
                if (uInt32PropertyComparison != 0)
                {
                    return uInt32PropertyComparison;
                }

                return x.UInt64Property.CompareTo(y.UInt64Property);
            }
        }
    }
}
