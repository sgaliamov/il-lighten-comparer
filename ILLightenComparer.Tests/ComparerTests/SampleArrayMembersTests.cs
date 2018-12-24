using System;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Samples.Comparers;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests
{
    public sealed class SampleArrayMembersTests
    {
        [Fact]
        public void Compare_Sample_Objects()
        {
            Test(typeof(SampleObject<>), typeof(SampleObjectComparer<>));
        }

        [Fact]
        public void Compare_Sample_Structs()
        {
            Test(typeof(SampleStruct<>), typeof(SampleStructComparer<>));
        }

        private static void Test(Type objectGenericType, Type genericCollectionType = null)
        {
            foreach (var item in SampleTypes.Types)
            {
                var arrayType = item.Key.MakeArrayType();
                var objectType = objectGenericType.MakeGenericType(arrayType);

                GenericTests.TestCollection(objectType, item.Value, genericCollectionType, false);
            }
        }
    }
}
