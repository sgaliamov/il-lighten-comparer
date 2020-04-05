using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ILLightenComparer.Tests.EqualityTests.EqualityComparers;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Utilities;

namespace ILLightenComparer.Tests.EqualityTests.Samples
{
    internal static class SampleTypes
    {
        static SampleTypes()
        {
            Types = new Dictionary<Type, IEqualityComparer> {
                [typeof(sbyte)] = null,
                [typeof(byte)] = null,
                [typeof(char)] = null,
                [typeof(short)] = null,
                [typeof(ushort)] = null,
                [typeof(int)] = null,
                [typeof(long)] = null,
                [typeof(ulong)] = null,
                [typeof(float)] = null,
                [typeof(double)] = null,
                [typeof(decimal)] = null,
                [typeof(EnumSmall)] = null,
                [typeof(EnumBig)] = null,
                [typeof(string)] = null,
                [typeof(SampleEqualityComparableBaseObject<EnumSmall?>)] = null,
                [typeof(SampleEqualityComparableChildObject<EnumSmall?>)] = null,
                [typeof(SampleEqualityComparableStruct<EnumSmall?>)] = null,
                [typeof(SampleObject<EnumSmall?>)] = new SampleObjectEqualityComparer<EnumSmall?>(),
                [typeof(SampleStruct<EnumSmall?>)] = new SampleStructEqualityComparer<EnumSmall?>()
            };

            NullableTypes = Types
                .Where(x => x.Key.IsValueType)
                .ToDictionary(x => x.Key.MakeNullable(), x => Helper.CreateNullableEqualityComparer(x.Key, x.Value));
        }

        public static IDictionary<Type, IEqualityComparer> NullableTypes { get; }

        public static IDictionary<Type, IEqualityComparer> Types { get; }
    }
}
