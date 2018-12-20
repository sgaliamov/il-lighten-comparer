using System.Collections.Generic;
using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.ComparerTests.IntegralTests
{
    public sealed class IntegralFieldsSampleObject
    {
        public byte ByteField;
        public char CharField;
        public EnumSmall EnumField;
        public sbyte SByteField;
        public short ShortField;
        public ushort UShortField;

        public static IComparer<IntegralFieldsSampleObject> Comparer { get; } =
            new RelationalComparer();

        private sealed class RelationalComparer : IComparer<IntegralFieldsSampleObject>
        {
            public int Compare(IntegralFieldsSampleObject x, IntegralFieldsSampleObject y)
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

                return 0;
            }
        }
    }
}
