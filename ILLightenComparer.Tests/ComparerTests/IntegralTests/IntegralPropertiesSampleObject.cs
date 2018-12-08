using System.Collections.Generic;
using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.ComparerTests.IntegralTests
{
    public class IntegralPropertiesSampleObject
    {
        public static IComparer<IntegralPropertiesSampleObject> Comparer { get; } =
            new RelationalComparer();

        public byte ByteProperty { get; set; }
        public char CharProperty { get; set; }
        public EnumSmall EnumProperty { get; set; }
        public sbyte SByteProperty { get; set; }
        public short ShortProperty { get; set; }
        public ushort UShortProperty { get; set; }

        private sealed class RelationalComparer : IComparer<IntegralPropertiesSampleObject>
        {
            public int Compare(IntegralPropertiesSampleObject x, IntegralPropertiesSampleObject y)
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
