using System;
using System.Collections;
using System.Threading.Tasks;
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
            Test(typeof(SampleObject<>), typeof(SampleObjectComparer<>), false);
            Test(typeof(SampleObject<>), typeof(SampleObjectComparer<>), true);
        }

        [Fact]
        public void Compare_Sample_Structs()
        {
            Test(typeof(SampleStruct<>), typeof(SampleStructComparer<>), false);
            Test(typeof(SampleStruct<>), typeof(SampleStructComparer<>), true);
        }

        private static void Test(Type objectGenericType, Type comparerGenericType, bool nullable)
        {
            var types = nullable ? SampleTypes.NullableTypes : SampleTypes.Types;

            Parallel.ForEach(
                types,
                item =>
                {
                    var (type, referenceComparer) = item;
                    var objectType = objectGenericType.MakeGenericType(type);
                    var comparerType = comparerGenericType.MakeGenericType(type);
                    var comparer = (IComparer)Activator.CreateInstance(comparerType, referenceComparer);

                    new GenericTests().GenericTest(objectType, comparer, false, Constants.SmallCount);
                });
        }
    }
}
