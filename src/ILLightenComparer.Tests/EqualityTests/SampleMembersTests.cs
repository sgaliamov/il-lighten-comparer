using System;
using System.Collections;
using System.Threading.Tasks;
using ILLightenComparer.Tests.EqualityComparers;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.EqualityTests
{
    public sealed class SampleMembersTests
    {
        [Fact]
        public void Compare_sample_objects()
        {
            Test(typeof(ComparableObject<>), typeof(ComparableObjectEqualityComparer<>), false);
            Test(typeof(ComparableObject<>), typeof(ComparableObjectEqualityComparer<>), true);
        }

        [Fact]
        public void Compare_sample_structs()
        {
            Test(typeof(ComparableStruct<>), typeof(ComparableStructEqualityComparer<>), false);
            Test(typeof(ComparableStruct<>), typeof(ComparableStructEqualityComparer<>), true);
        }

        private static void Test(Type genericSampleType, Type genericSampleComparer, bool nullable)
        {
            var types = nullable ? SampleTypes.NullableTypes : SampleTypes.Types;

            Parallel.ForEach(
                types,
                item => {
                    var (type, referenceComparer) = item;
                    var objectType = genericSampleType.MakeGenericType(type);
                    var comparerType = genericSampleComparer.MakeGenericType(type);
                    var comparer = (IEqualityComparer)Activator.CreateInstance(comparerType, referenceComparer);

                    new GenericTests().GenericTest(objectType, comparer, false, Constants.SmallCount);
                });
        }
    }
}
