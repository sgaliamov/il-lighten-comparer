using System;
using System.Reflection;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests
{
    public sealed class SampleComparableTests
    {
        [Fact]
        public void Compare_Sample_Comparable_Objects()
        {
            Test(typeof(SampleComparableBaseObject<>), nameof(SampleComparableBaseObject<object>.Comparer));
            Test(typeof(SampleComparableChildObject<>), nameof(SampleComparableChildObject<object>.ChildComparer));
        }

        [Fact]
        public void Compare_Sample_Comparable_Structs()
        {
            Test(typeof(SampleComparableStruct<>), nameof(SampleComparableStruct<object>.Comparer));
        }

        private static void Test(Type objectGenericType, string comparerName)
        {
            foreach (var item in SampleTypes.Types)
            {
                var objectType = objectGenericType.MakeGenericType(item.Key);

                if (item.Value != null)
                {
                    objectType
                        .GetField(comparerName, BindingFlags.Public | BindingFlags.Static)
                        .SetValue(null, item.Value);
                }


                GenericTests.GenericTest(objectType, null, false, Constants.SmallCount);
            }
        }
    }
}
