using System;
using System.Collections;
using System.Collections.Generic;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Samples.Comparers;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests
{
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
        public void Compare_Nullable_Types_Directly()
        {
            foreach (var (nullableType, nullableComparer) in SampleTypes.NullableTypes)
            {
                new GenericTests().GenericTest(nullableType, nullableComparer, false, Constants.SmallCount);
            }
        }

        [Fact]
        public void Compare_Types_Directly()
        {
            foreach (var (type, referenceComparer) in SampleTypes.Types)
            {
                new GenericTests().GenericTest(type, referenceComparer, false, Constants.SmallCount);
            }
        }

        private static void TestCollection(Type genericCollectionType = null)
        {
            foreach (var (type, comparer) in SampleTypes.Types)
            {
                TestCollection(type, comparer, genericCollectionType, false);
                TestCollection(type, comparer, genericCollectionType, true);
            }
        }

        private static void TestNullableCollection(Type genericCollectionType = null)
        {
            foreach (var (nullableType, nullableComparer) in SampleTypes.NullableTypes)
            {
                TestCollection(nullableType, nullableComparer, genericCollectionType, false);
                TestCollection(nullableType, nullableComparer, genericCollectionType, true);
            }
        }

        private static void TestCollection(
            Type objectType,
            IComparer itemComparer,
            Type genericCollectionType,
            bool sort)
        {
            var collectionType = genericCollectionType == null
                                     ? objectType.MakeArrayType()
                                     : genericCollectionType.MakeGenericType(objectType);

            var comparerType = typeof(CollectionComparer<>).MakeGenericType(objectType);
            var constructor = comparerType.GetConstructor(new[] { typeof(IComparer<>).MakeGenericType(objectType), typeof(bool) });
            var comparer = (IComparer)constructor.Invoke(new object[] { itemComparer, sort });

            new GenericTests().GenericTest(collectionType, comparer, sort, Constants.SmallCount);
        }
    }
}
