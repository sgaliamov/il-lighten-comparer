using System;
using System.Collections;
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

        public static IComparer<TestObject> TestObjectComparer { get; } = new TestObjectRelationalComparer();

        public override string ToString() =>
            $"{nameof(BooleanProperty)}: {BooleanProperty}, {nameof(ByteProperty)}: {ByteProperty}, {nameof(SByteProperty)}: {SByteProperty}, {nameof(CharProperty)}: {CharProperty}, {nameof(DecimalProperty)}: {DecimalProperty}, {nameof(DoubleProperty)}: {DoubleProperty}, {nameof(SingleProperty)}: {SingleProperty}, {nameof(Int32Property)}: {Int32Property}, {nameof(UInt32Property)}: {UInt32Property}, {nameof(Int64Property)}: {Int64Property}, {nameof(UInt64Property)}: {UInt64Property}, {nameof(Int16Property)}: {Int16Property}, {nameof(UInt16Property)}: {UInt16Property}, {nameof(StringProperty)}: {StringProperty}";

        private sealed class TestObjectRelationalComparer :
            IComparer<TestObject>,
            IComparer
        {
            public int Compare(object x, object y) => Compare((TestObject)x, (TestObject)y);

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

                return string.Compare(x.StringProperty, y.StringProperty, StringComparison.Ordinal);
            }
        }
    }
}
