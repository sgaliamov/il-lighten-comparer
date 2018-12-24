using System;
using System.Collections.Generic;
using System.Linq;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests
{
    // todo: test with interface, abstract class and object
    public sealed class SampleTypesTests
    {
        [Fact]
        public void Compare_Arrays_Directly()
        {
            TestCollection();
        }

        [Fact]
        public void Compare_Arrays_Of_Nullables_Directly()
        {
            TestNullableCollection();
        }

        [Fact]
        public void Compare_Enumerables_Directly()
        {
            TestCollection(typeof(IEnumerable<>));
        }

        [Fact]
        public void Compare_Enumerables_Of_Nullables_Directly()
        {
            TestNullableCollection(typeof(IEnumerable<>));
        }

        [Fact]
        public void Compare_Types_Directly()
        {
            foreach (var item in SampleTypes.Types)
            {
                GenericTests.GenericTest(item.Key, item.Value, Constants.SmallCount);
            }
        }

        private static void TestCollection(Type genericCollectionType = null)
        {
            foreach (var item in SampleTypes.Types)
            {
                GenericTests.TestCollection(item.Key, item.Value, genericCollectionType, false);
            }
        }

        private static void TestNullableCollection(Type genericCollectionType = null)
        {
            foreach (var item in SampleTypes.Types.Where(x => x.Key.IsValueType))
            {
                GenericTests.TestCollection(item.Key, item.Value, genericCollectionType, true);
            }
        }
    }
}
