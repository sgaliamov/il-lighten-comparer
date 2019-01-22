using System;
using System.Collections;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Samples.Comparers;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests
{
    public sealed class SampleMembersTests
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

        private static void Test(Type objectGenericType, Type comparerGenericType)
        {
            foreach (var item in SampleTypes.Types)
            {
                var objectType = objectGenericType.MakeGenericType(item.Key);
                var comparerType = comparerGenericType.MakeGenericType(item.Key);
                var comparer = (IComparer)Activator.CreateInstance(comparerType, item.Value);

                new GenericTests().GenericTest(objectType, comparer, false, Constants.SmallCount);
            }
        }
    }
}
