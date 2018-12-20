using System;
using System.Collections;
using System.Collections.Generic;
using ILLightenComparer.Tests.ComparerTests.ComparableTests.Samples;
using ILLightenComparer.Tests.Samples.Comparers;

namespace ILLightenComparer.Tests.Samples
{
    internal static class SampleTypes
    {
        static SampleTypes()
        {
            Types = new Dictionary<Type, IComparer>
            {
                { typeof(byte), null },
                { typeof(ComparableStruct<EnumSmall>), null },
                { typeof(ComparableObject), null },
                { typeof(ComparableChildObject), null },
                { typeof(EnumSmall), null },
                { typeof(EnumBig), null },
                { typeof(long), null },
                { typeof(ulong), null },
                { typeof(decimal), null },
                { typeof(string), StringComparer.Ordinal },
                {
                    typeof(SampleObject<SampleObject<int>>),
                    new SampleObjectComparer<SampleObject<int>>(new SampleObjectComparer<int>())
                },
                {
                    typeof(SampleObject<int>),
                    new SampleObjectComparer<int>()
                },
                {
                    typeof(SampleStruct<int>),
                    new SampleStructComparer<int>()
                }
            };
        }

        public static IDictionary<Type, IComparer> Types { get; }
    }
}
