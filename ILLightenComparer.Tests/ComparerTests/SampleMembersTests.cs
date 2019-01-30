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
            foreach (var (type, referenceComparer) in SampleTypes.Types)
            {
                var objectType = objectGenericType.MakeGenericType(type);
                var comparerType = comparerGenericType.MakeGenericType(type);
                var comparer = (IComparer)Activator.CreateInstance(comparerType, referenceComparer);

                new GenericTests().GenericTest(objectType, comparer, false, Constants.SmallCount);
            }
        }
    }
}
