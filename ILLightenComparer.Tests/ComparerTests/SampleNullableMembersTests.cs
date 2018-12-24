using System;
using System.Collections;
using System.Linq;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Samples.Comparers;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests
{
    public sealed class SampleNullableMembersTests
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
            foreach (var item in SampleTypes.Types.Where(x => x.Key.IsValueType))
            {
                var nullableComparerType = typeof(NullableComparer<>).MakeGenericType(item.Key);
                var nullableComparer = (IComparer)Activator.CreateInstance(nullableComparerType, item.Value);
                var nullableType = typeof(Nullable<>).MakeGenericType(item.Key);
                var objectType = objectGenericType.MakeGenericType(nullableType);

                var comparerType = comparerGenericType.MakeGenericType(nullableType);
                var comparer = (IComparer)Activator.CreateInstance(comparerType, nullableComparer);

                GenericTests.GenericTest(objectType, comparer, false, Constants.SmallCount);
            }
        }
    }
}
