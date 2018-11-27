﻿using System.Collections.Generic;
using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.ComparerTests.IntegralTests
{
    public class IntegralSampleObject
    {
        public byte ByteField;
        public char CharField;
        public EnumSmall EnumField;
        public sbyte SByteField;
        public short ShortField;
        public ushort UShortField;

        public static IComparer<IntegralSampleObject> Comparer { get; } =
            new IntegralSampleObjectRelationalComparer();

        public byte ByteProperty { get; set; }
        public char CharProperty { get; set; }
        public EnumSmall EnumProperty { get; set; }
        public sbyte SByteProperty { get; set; }
        public short ShortProperty { get; set; }
        public ushort UShortProperty { get; set; }

        private sealed class IntegralSampleObjectRelationalComparer : IComparer<IntegralSampleObject>
        {
            public int Compare(IntegralSampleObject x, IntegralSampleObject y)
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

                var enumFieldComparison = x.EnumField.CompareTo(y.EnumField);
                if (enumFieldComparison != 0)
                {
                    return enumFieldComparison;
                }

                var sByteFieldComparison = x.SByteField.CompareTo(y.SByteField);
                if (sByteFieldComparison != 0)
                {
                    return sByteFieldComparison;
                }

                var shortFieldComparison = x.ShortField.CompareTo(y.ShortField);
                if (shortFieldComparison != 0)
                {
                    return shortFieldComparison;
                }

                var uShortFieldComparison = x.UShortField.CompareTo(y.UShortField);
                if (uShortFieldComparison != 0)
                {
                    return uShortFieldComparison;
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

                var enumPropertyComparison = x.EnumProperty.CompareTo(y.EnumProperty);
                if (enumPropertyComparison != 0)
                {
                    return enumPropertyComparison;
                }

                var sBytePropertyComparison = x.SByteProperty.CompareTo(y.SByteProperty);
                if (sBytePropertyComparison != 0)
                {
                    return sBytePropertyComparison;
                }

                var shortPropertyComparison = x.ShortProperty.CompareTo(y.ShortProperty);
                if (shortPropertyComparison != 0)
                {
                    return shortPropertyComparison;
                }

                return x.UShortProperty.CompareTo(y.UShortProperty);
            }
        }
    }
}