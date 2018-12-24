using System;
using System.Collections;
using System.Collections.Generic;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Samples.Comparers;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests
{
    public sealed class SampleArrayMembersTests
    {
        [Fact]
        public void Compare_Sample_Objects()
        {
            Test(typeof(SampleObject<>), typeof(SampleObjectComparer<>), true, false);
        }

        [Fact]
        public void Compare_Sample_Structs()
        {
            Test(typeof(SampleStruct<>), typeof(SampleStructComparer<>), true, false);
        }

        private static void Test(Type genericSampleType, Type genericSampleComparer, bool useArrays, bool sort)
        {
            foreach (var item in SampleTypes.Types)
            {
                var collectionType = useArrays
                                         ? item.Key.MakeArrayType()
                                         : typeof(IEnumerable<>).MakeGenericType(item.Key);

                var objectType = genericSampleType.MakeGenericType(collectionType);

                var collectionComparerType = typeof(CollectionComparer<,>).MakeGenericType(collectionType, item.Key);
                var collectionComparer = Activator.CreateInstance(collectionComparerType, item.Value, sort);

                var sampleComparerType = genericSampleComparer.MakeGenericType(collectionType);
                var sampleComparer = (IComparer)Activator.CreateInstance(sampleComparerType, collectionComparer);

                GenericTests.GenericTest(objectType, sampleComparer, Constants.SmallCount);
            }
        }
    }
}
