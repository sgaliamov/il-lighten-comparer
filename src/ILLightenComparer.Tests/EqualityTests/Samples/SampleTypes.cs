using System;
using System.Collections;
using System.Collections.Generic;
using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.EqualityTests.Samples
{
    internal static class SampleTypes
    {
        static SampleTypes()
        {
            Types = new Dictionary<Type, IEqualityComparer> {
                // ceq
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
                // ==
                { typeof(string), null },
                // equals
                //{ typeof(SampleComparableBaseObject<EnumSmall?>), null },
                //{ typeof(SampleComparableChildObject<EnumSmall?>), null },
                //{ typeof(SampleComparableStruct<EnumSmall?>), null }, 
                // compiled
                // {
                //    typeof(SampleObject<EnumSmall?>),
                //    new SampleObjectComparer<EnumSmall?>()
                //}, {
                //    typeof(SampleStruct<EnumSmall?>),
                //    new SampleStructComparer<EnumSmall?>()
                //}
            };

            //NullableTypes = Types.Where(x => x.Key.IsValueType)
            //                     .ToDictionary(
            //                         x => x.Key.MakeNullable(),
            //                         x => Helper.CreateNullableComparer(x.Key, x.Value));
        }

        //public static IDictionary<Type, IEqualityComparer> NullableTypes { get; }

        public static IDictionary<Type, IEqualityComparer> Types { get; }
    }
}
