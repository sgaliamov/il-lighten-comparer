using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ILLightenComparer.Tests.EqualityComparers;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.EqualityTests
{
    public sealed class SampleTypesTests
    {
        [Fact]
        public void Compare_arrays_directly() => TestCollection();

        [Fact]
        public void Compare_arrays_of_nullables_directly() => TestNullableCollection();

        [Fact]
        public void Compare_enumerables_directly() => TestCollection(typeof(IEnumerable<>));

        [Fact]
        public void Compare_enumerables_of_nullables_directly() => TestNullableCollection(typeof(IEnumerable<>));

        [Fact]
        public void Compare_nullable_types_directly()
        {
            foreach (var (nullableType, nullableComparer) in TestTypes.NullableTypes) {
                new GenericTests(false).GenericTest(nullableType, nullableComparer, Constants.SmallCount);
            }
        }

        [Fact]
        public void Compare_types_directly()
        {
            Parallel.ForEach(TestTypes.Types, item => {
                var (type, referenceComparer) = item;
                new GenericTests(false).GenericTest(type, referenceComparer, Constants.SmallCount);
            });
        }

        private static void TestCollection(Type genericCollectionType = null)
        {
            Parallel.ForEach(TestTypes.Types, item => {
                var (type, referenceComparer) = item;
                TestCollection(type, referenceComparer, genericCollectionType, false);
                TestCollection(type, referenceComparer, genericCollectionType, true);
            });
        }

        private static void TestNullableCollection(Type genericCollectionType = null)
        {
            foreach (var (nullableType, nullableComparer) in TestTypes.NullableTypes) {
                TestCollection(nullableType, nullableComparer, genericCollectionType, false);
                TestCollection(nullableType, nullableComparer, genericCollectionType, true);
            }
        }

        private static void TestCollection(Type objectType, IEqualityComparer itemComparer, Type genericCollectionType, bool sort)
        {
            var collectionType = genericCollectionType == null
                ? objectType.MakeArrayType()
                : genericCollectionType.MakeGenericType(objectType);

            var comparerType = typeof(CollectionEqualityComparer<>).MakeGenericType(objectType);
            var constructor = comparerType.GetConstructor(new[] { typeof(IEqualityComparer<>).MakeGenericType(objectType), typeof(bool), typeof(IComparer<>).MakeGenericType(objectType) });
            var comparer = (IEqualityComparer)constructor.Invoke(new object[] { itemComparer, sort, null });

            new GenericTests(sort).GenericTest(collectionType, comparer, Constants.SmallCount);
        }
    }
}
