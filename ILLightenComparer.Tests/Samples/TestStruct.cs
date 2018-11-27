using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.Samples
{
    public struct TestStruct
    {
        public EnumBig BigEnumField;
        public EnumBig? BigEnumNullableField;
        public bool BooleanField;
        public bool? BooleanNullableField;
        public byte ByteField;
        public byte? ByteNullableField;
        public char CharField;
        public char? CharNullableField;
        public decimal DecimalField;
        public decimal? DecimalNullableField;
        public double DoubleField;
        public double? DoubleNullableField;
        public short Int16Field;
        public short? Int16NullableField;
        public int Int32Field;
        public int? Int32NullableField;
        public long Int64Field;
        public long? Int64NullableField;
        public sbyte SByteField;
        public sbyte? SByteNullableField;
        public float SingleField;
        public float? SingleNullableField;
        public EnumSmall SmallEnumField;
        public EnumSmall? SmallEnumNullableField;
        public string StringField;
        public ushort UInt16Field;
        public ushort? UInt16NullableField;
        public uint UInt32Field;
        public uint? UInt32NullableField;
        public ulong UInt64Field;
        public ulong? UInt64NullableField;

        public static IComparer<TestStruct> TestStructComparer { get; } = new TestStructRelationalComparer();

        public EnumBig? BigEnumNullableProperty { get; set; }
        public EnumBig BigEnumProperty { get; set; }
        public bool? BooleanNullableProperty { get; set; }
        public bool BooleanProperty { get; set; }
        public byte? ByteNullableProperty { get; set; }
        public byte ByteProperty { get; set; }
        public char? CharNullableProperty { get; set; }
        public char CharProperty { get; set; }
        public decimal? DecimalNullableProperty { get; set; }
        public decimal DecimalProperty { get; set; }
        public double? DoubleNullableProperty { get; set; }
        public double DoubleProperty { get; set; }
        public short? Int16NullableProperty { get; set; }
        public short Int16Property { get; set; }
        public int? Int32NullableProperty { get; set; }
        public int Int32Property { get; set; }
        public long? Int64NullableProperty { get; set; }
        public long Int64Property { get; set; }
        public sbyte? SByteNullableProperty { get; set; }
        public sbyte SByteProperty { get; set; }
        public float? SingleNullableProperty { get; set; }
        public float SingleProperty { get; set; }
        public EnumSmall? SmallEnumNullableProperty { get; set; }
        public EnumSmall SmallEnumProperty { get; set; }
        public string StringProperty { get; set; }
        public ushort? UInt16NullableProperty { get; set; }
        public ushort UInt16Property { get; set; }
        public uint? UInt32NullableProperty { get; set; }
        public uint UInt32Property { get; set; }
        public ulong? UInt64NullableProperty { get; set; }
        public ulong UInt64Property { get; set; }

        private sealed class TestStructRelationalComparer : IComparer<TestStruct>
        {
            public int Compare(TestStruct x, TestStruct y)
            {
                var bigEnumFieldComparison = x.BigEnumField.CompareTo(y.BigEnumField);
                if (bigEnumFieldComparison != 0)
                {
                    return bigEnumFieldComparison;
                }

                var bigEnumNullableFieldComparison = Nullable.Compare(x.BigEnumNullableField, y.BigEnumNullableField);
                if (bigEnumNullableFieldComparison != 0)
                {
                    return bigEnumNullableFieldComparison;
                }

                var booleanFieldComparison = x.BooleanField.CompareTo(y.BooleanField);
                if (booleanFieldComparison != 0)
                {
                    return booleanFieldComparison;
                }

                var booleanNullableFieldComparison = Nullable.Compare(x.BooleanNullableField, y.BooleanNullableField);
                if (booleanNullableFieldComparison != 0)
                {
                    return booleanNullableFieldComparison;
                }

                var byteFieldComparison = x.ByteField.CompareTo(y.ByteField);
                if (byteFieldComparison != 0)
                {
                    return byteFieldComparison;
                }

                var byteNullableFieldComparison = Nullable.Compare(x.ByteNullableField, y.ByteNullableField);
                if (byteNullableFieldComparison != 0)
                {
                    return byteNullableFieldComparison;
                }

                var charFieldComparison = x.CharField.CompareTo(y.CharField);
                if (charFieldComparison != 0)
                {
                    return charFieldComparison;
                }

                var charNullableFieldComparison = Nullable.Compare(x.CharNullableField, y.CharNullableField);
                if (charNullableFieldComparison != 0)
                {
                    return charNullableFieldComparison;
                }

                var decimalFieldComparison = x.DecimalField.CompareTo(y.DecimalField);
                if (decimalFieldComparison != 0)
                {
                    return decimalFieldComparison;
                }

                var decimalNullableFieldComparison = Nullable.Compare(x.DecimalNullableField, y.DecimalNullableField);
                if (decimalNullableFieldComparison != 0)
                {
                    return decimalNullableFieldComparison;
                }

                var doubleFieldComparison = x.DoubleField.CompareTo(y.DoubleField);
                if (doubleFieldComparison != 0)
                {
                    return doubleFieldComparison;
                }

                var doubleNullableFieldComparison = Nullable.Compare(x.DoubleNullableField, y.DoubleNullableField);
                if (doubleNullableFieldComparison != 0)
                {
                    return doubleNullableFieldComparison;
                }

                var int16FieldComparison = x.Int16Field.CompareTo(y.Int16Field);
                if (int16FieldComparison != 0)
                {
                    return int16FieldComparison;
                }

                var int16NullableFieldComparison = Nullable.Compare(x.Int16NullableField, y.Int16NullableField);
                if (int16NullableFieldComparison != 0)
                {
                    return int16NullableFieldComparison;
                }

                var int32FieldComparison = x.Int32Field.CompareTo(y.Int32Field);
                if (int32FieldComparison != 0)
                {
                    return int32FieldComparison;
                }

                var int32NullableFieldComparison = Nullable.Compare(x.Int32NullableField, y.Int32NullableField);
                if (int32NullableFieldComparison != 0)
                {
                    return int32NullableFieldComparison;
                }

                var int64FieldComparison = x.Int64Field.CompareTo(y.Int64Field);
                if (int64FieldComparison != 0)
                {
                    return int64FieldComparison;
                }

                var int64NullableFieldComparison = Nullable.Compare(x.Int64NullableField, y.Int64NullableField);
                if (int64NullableFieldComparison != 0)
                {
                    return int64NullableFieldComparison;
                }

                var sByteFieldComparison = x.SByteField.CompareTo(y.SByteField);
                if (sByteFieldComparison != 0)
                {
                    return sByteFieldComparison;
                }

                var sByteNullableFieldComparison = Nullable.Compare(x.SByteNullableField, y.SByteNullableField);
                if (sByteNullableFieldComparison != 0)
                {
                    return sByteNullableFieldComparison;
                }

                var singleFieldComparison = x.SingleField.CompareTo(y.SingleField);
                if (singleFieldComparison != 0)
                {
                    return singleFieldComparison;
                }

                var singleNullableFieldComparison = Nullable.Compare(x.SingleNullableField, y.SingleNullableField);
                if (singleNullableFieldComparison != 0)
                {
                    return singleNullableFieldComparison;
                }

                var smallEnumFieldComparison = x.SmallEnumField.CompareTo(y.SmallEnumField);
                if (smallEnumFieldComparison != 0)
                {
                    return smallEnumFieldComparison;
                }

                var smallEnumNullableFieldComparison =
                    Nullable.Compare(x.SmallEnumNullableField, y.SmallEnumNullableField);
                if (smallEnumNullableFieldComparison != 0)
                {
                    return smallEnumNullableFieldComparison;
                }

                var stringFieldComparison = string.Compare(x.StringField, y.StringField, StringComparison.Ordinal);
                if (stringFieldComparison != 0)
                {
                    return stringFieldComparison;
                }

                var uInt16FieldComparison = x.UInt16Field.CompareTo(y.UInt16Field);
                if (uInt16FieldComparison != 0)
                {
                    return uInt16FieldComparison;
                }

                var uInt16NullableFieldComparison = Nullable.Compare(x.UInt16NullableField, y.UInt16NullableField);
                if (uInt16NullableFieldComparison != 0)
                {
                    return uInt16NullableFieldComparison;
                }

                var uInt32FieldComparison = x.UInt32Field.CompareTo(y.UInt32Field);
                if (uInt32FieldComparison != 0)
                {
                    return uInt32FieldComparison;
                }

                var uInt32NullableFieldComparison = Nullable.Compare(x.UInt32NullableField, y.UInt32NullableField);
                if (uInt32NullableFieldComparison != 0)
                {
                    return uInt32NullableFieldComparison;
                }

                var uInt64FieldComparison = x.UInt64Field.CompareTo(y.UInt64Field);
                if (uInt64FieldComparison != 0)
                {
                    return uInt64FieldComparison;
                }

                var uInt64NullableFieldComparison = Nullable.Compare(x.UInt64NullableField, y.UInt64NullableField);
                if (uInt64NullableFieldComparison != 0)
                {
                    return uInt64NullableFieldComparison;
                }

                var bigEnumNullablePropertyComparison =
                    Nullable.Compare(x.BigEnumNullableProperty, y.BigEnumNullableProperty);
                if (bigEnumNullablePropertyComparison != 0)
                {
                    return bigEnumNullablePropertyComparison;
                }

                var bigEnumPropertyComparison = x.BigEnumProperty.CompareTo(y.BigEnumProperty);
                if (bigEnumPropertyComparison != 0)
                {
                    return bigEnumPropertyComparison;
                }

                var booleanNullablePropertyComparison =
                    Nullable.Compare(x.BooleanNullableProperty, y.BooleanNullableProperty);
                if (booleanNullablePropertyComparison != 0)
                {
                    return booleanNullablePropertyComparison;
                }

                var booleanPropertyComparison = x.BooleanProperty.CompareTo(y.BooleanProperty);
                if (booleanPropertyComparison != 0)
                {
                    return booleanPropertyComparison;
                }

                var byteNullablePropertyComparison = Nullable.Compare(x.ByteNullableProperty, y.ByteNullableProperty);
                if (byteNullablePropertyComparison != 0)
                {
                    return byteNullablePropertyComparison;
                }

                var bytePropertyComparison = x.ByteProperty.CompareTo(y.ByteProperty);
                if (bytePropertyComparison != 0)
                {
                    return bytePropertyComparison;
                }

                var charNullablePropertyComparison = Nullable.Compare(x.CharNullableProperty, y.CharNullableProperty);
                if (charNullablePropertyComparison != 0)
                {
                    return charNullablePropertyComparison;
                }

                var charPropertyComparison = x.CharProperty.CompareTo(y.CharProperty);
                if (charPropertyComparison != 0)
                {
                    return charPropertyComparison;
                }

                var decimalNullablePropertyComparison =
                    Nullable.Compare(x.DecimalNullableProperty, y.DecimalNullableProperty);
                if (decimalNullablePropertyComparison != 0)
                {
                    return decimalNullablePropertyComparison;
                }

                var decimalPropertyComparison = x.DecimalProperty.CompareTo(y.DecimalProperty);
                if (decimalPropertyComparison != 0)
                {
                    return decimalPropertyComparison;
                }

                var doubleNullablePropertyComparison =
                    Nullable.Compare(x.DoubleNullableProperty, y.DoubleNullableProperty);
                if (doubleNullablePropertyComparison != 0)
                {
                    return doubleNullablePropertyComparison;
                }

                var doublePropertyComparison = x.DoubleProperty.CompareTo(y.DoubleProperty);
                if (doublePropertyComparison != 0)
                {
                    return doublePropertyComparison;
                }

                var int16NullablePropertyComparison =
                    Nullable.Compare(x.Int16NullableProperty, y.Int16NullableProperty);
                if (int16NullablePropertyComparison != 0)
                {
                    return int16NullablePropertyComparison;
                }

                var int16PropertyComparison = x.Int16Property.CompareTo(y.Int16Property);
                if (int16PropertyComparison != 0)
                {
                    return int16PropertyComparison;
                }

                var int32NullablePropertyComparison =
                    Nullable.Compare(x.Int32NullableProperty, y.Int32NullableProperty);
                if (int32NullablePropertyComparison != 0)
                {
                    return int32NullablePropertyComparison;
                }

                var int32PropertyComparison = x.Int32Property.CompareTo(y.Int32Property);
                if (int32PropertyComparison != 0)
                {
                    return int32PropertyComparison;
                }

                var int64NullablePropertyComparison =
                    Nullable.Compare(x.Int64NullableProperty, y.Int64NullableProperty);
                if (int64NullablePropertyComparison != 0)
                {
                    return int64NullablePropertyComparison;
                }

                var int64PropertyComparison = x.Int64Property.CompareTo(y.Int64Property);
                if (int64PropertyComparison != 0)
                {
                    return int64PropertyComparison;
                }

                var sByteNullablePropertyComparison =
                    Nullable.Compare(x.SByteNullableProperty, y.SByteNullableProperty);
                if (sByteNullablePropertyComparison != 0)
                {
                    return sByteNullablePropertyComparison;
                }

                var sBytePropertyComparison = x.SByteProperty.CompareTo(y.SByteProperty);
                if (sBytePropertyComparison != 0)
                {
                    return sBytePropertyComparison;
                }

                var singleNullablePropertyComparison =
                    Nullable.Compare(x.SingleNullableProperty, y.SingleNullableProperty);
                if (singleNullablePropertyComparison != 0)
                {
                    return singleNullablePropertyComparison;
                }

                var singlePropertyComparison = x.SingleProperty.CompareTo(y.SingleProperty);
                if (singlePropertyComparison != 0)
                {
                    return singlePropertyComparison;
                }

                var smallEnumNullablePropertyComparison =
                    Nullable.Compare(x.SmallEnumNullableProperty, y.SmallEnumNullableProperty);
                if (smallEnumNullablePropertyComparison != 0)
                {
                    return smallEnumNullablePropertyComparison;
                }

                var smallEnumPropertyComparison = x.SmallEnumProperty.CompareTo(y.SmallEnumProperty);
                if (smallEnumPropertyComparison != 0)
                {
                    return smallEnumPropertyComparison;
                }

                var stringPropertyComparison =
                    string.Compare(x.StringProperty, y.StringProperty, StringComparison.Ordinal);
                if (stringPropertyComparison != 0)
                {
                    return stringPropertyComparison;
                }

                var uInt16NullablePropertyComparison =
                    Nullable.Compare(x.UInt16NullableProperty, y.UInt16NullableProperty);
                if (uInt16NullablePropertyComparison != 0)
                {
                    return uInt16NullablePropertyComparison;
                }

                var uInt16PropertyComparison = x.UInt16Property.CompareTo(y.UInt16Property);
                if (uInt16PropertyComparison != 0)
                {
                    return uInt16PropertyComparison;
                }

                var uInt32NullablePropertyComparison =
                    Nullable.Compare(x.UInt32NullableProperty, y.UInt32NullableProperty);
                if (uInt32NullablePropertyComparison != 0)
                {
                    return uInt32NullablePropertyComparison;
                }

                var uInt32PropertyComparison = x.UInt32Property.CompareTo(y.UInt32Property);
                if (uInt32PropertyComparison != 0)
                {
                    return uInt32PropertyComparison;
                }

                var uInt64NullablePropertyComparison =
                    Nullable.Compare(x.UInt64NullableProperty, y.UInt64NullableProperty);
                if (uInt64NullablePropertyComparison != 0)
                {
                    return uInt64NullablePropertyComparison;
                }

                return x.UInt64Property.CompareTo(y.UInt64Property);
            }
        }
    }
}
