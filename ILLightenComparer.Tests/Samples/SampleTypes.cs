using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ILLightenComparer.Tests.Samples.Comparers;
using ILLightenComparer.Tests.Utilities;

namespace ILLightenComparer.Tests.Samples
{
    internal static class SampleTypes
    {
        static SampleTypes()
        {
            Types = new Dictionary<Type, IComparer>
            {
                { typeof(sbyte), null },
                { typeof(byte), null },
                { typeof(char), null },
                { typeof(short), null },
                { typeof(ushort), null },
                { typeof(int), null },
                { typeof(long), null },
                { typeof(ulong), null },
                { typeof(float), null },
                { typeof(double), null },
                { typeof(decimal), null },
                { typeof(EnumSmall), null },
                { typeof(EnumBig), null },
                { typeof(string), null },
                { typeof(SampleComparableBaseObject<EnumSmall?>), null },
                { typeof(SampleComparableChildObject<EnumSmall?>), null },
                { typeof(SampleComparableStruct<EnumSmall?>), null },
                {
                    typeof(SampleObject<EnumSmall?>),
                    new SampleObjectComparer<EnumSmall?>()
                },
                {
                    typeof(SampleStruct<EnumSmall?>),
                    new SampleStructComparer<EnumSmall?>()
                }
            };

            NullableTypes = Types.Where(x => x.Key.IsValueType)
                                 .ToDictionary(
                                     x => x.Key.MakeNullable(),
                                     x => Helper.CreateNullableComparer(x.Key, x.Value));
        }

        public static IDictionary<Type, IComparer> NullableTypes { get; }

        public static IDictionary<Type, IComparer> Types { get; }
    }
}
