using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using ILLightenComparer.Tests.EqualityComparers;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.EqualityTests
{
    public sealed class CollectionOfCollectionsTests
    {
        private readonly IFixture _fixture = FixtureBuilder.GetInstance();

        [Fact]
        public void Compare_array_of_array()
        {
            Type[] GetCollectionTypes(Type type)
            {
                var array1Type = type.MakeArrayType();
                var array2Type = array1Type.MakeArrayType();

                return new[] { array1Type, array2Type };
            }

            CompareCollectionOfCollections(GetCollectionTypes);
        }

        [Fact]
        public void Compare_enumerables_of_enumerables()
        {
            var collectionComparer = new CollectionEqualityComparer<List<int?>>(new CollectionEqualityComparer<int?>());
            var referenceComparer = new CollectionEqualityComparer<EnumerableStruct<List<int?>>?>(
                new NullableEqualityComparer<EnumerableStruct<List<int?>>>(
                    new CustomizableEqualityComparer<EnumerableStruct<List<int?>>>(
                        (a, b) => collectionComparer.Equals(a, b),
                        x => collectionComparer.GetHashCode(x))));
            var comparer = new ComparerBuilder().GetEqualityComparer<IEnumerable<EnumerableStruct<List<int?>>?>>();

            Helper.Parallel(() => {
                var x = _fixture.CreateMany<EnumerableStruct<List<int?>>?>().RandomNulls().ToList();
                var y = _fixture.CreateMany<EnumerableStruct<List<int?>>?>().RandomNulls().ToList();

                var expectedEquals = referenceComparer.Equals(x, y);
                var expectedHashX = referenceComparer.GetHashCode(x);
                var expectedHashY = referenceComparer.GetHashCode(y);

                var equals = comparer.Equals(x, y);
                var hashX = comparer.GetHashCode(x);
                var hashY = comparer.GetHashCode(y);

                using (new AssertionScope()) {
                    comparer.Equals(x, x).Should().BeTrue();
                    equals.Should().Be(expectedEquals);
                    hashX.Should().Be(expectedHashX);
                    hashY.Should().Be(expectedHashY);
                }
            }, 1);
        }

        [Fact]
        public void Compare_array_of_list()
        {
            Type[] GetCollectionTypes(Type type)
            {
                var arrayType = type.MakeArrayType();
                var listType = typeof(List<>).MakeGenericType(arrayType);

                return new[] { arrayType, listType };
            }

            CompareCollectionOfCollections(GetCollectionTypes);
        }

        [Fact]
        public void Compare_list_of_array()
        {
            Type[] GetCollectionTypes(Type type)
            {
                var listType = typeof(List<>).MakeGenericType(type);
                var arrayType = listType.MakeArrayType();

                return new[] { listType, arrayType };
            }

            CompareCollectionOfCollections(GetCollectionTypes);
        }

        [Fact]
        public void Compare_list_of_list()
        {
            Type[] GetCollectionTypes(Type type)
            {
                var list1Type = typeof(List<>).MakeGenericType(type);
                var list2Type = typeof(List<>).MakeGenericType(list1Type);

                return new[] { list1Type, list2Type };
            }

            CompareCollectionOfCollections(GetCollectionTypes);
        }

        [Fact]
        public void Compare_multi_arrays()
        {
            var builder = new ComparerBuilder();

            Assert.Throws<NotSupportedException>(() => builder.For<ComparableObject<IEnumerable<int[,]>>>().GetComparer());
            Assert.Throws<NotSupportedException>(() => builder.For<ComparableStruct<IEnumerable<int[,]>>>().GetComparer());

            Assert.Throws<NotSupportedException>(() => builder.For<ComparableObject<int[][,]>>().GetComparer());
            Assert.Throws<NotSupportedException>(() => builder.For<ComparableObject<int[,]>>().GetComparer());

            Assert.Throws<NotSupportedException>(() => builder.For<ComparableStruct<int[][,]>>().GetComparer());
            Assert.Throws<NotSupportedException>(() => builder.For<ComparableStruct<int[,]>>().GetComparer());
        }

        private static void CompareCollectionOfCollections(Func<Type, Type[]> getCollectionTypes)
        {
            Parallel.Invoke(
                () => CompareCollectionOfCollections(getCollectionTypes, null, null),
                () => CompareCollectionOfCollections(getCollectionTypes, typeof(ComparableObject<>), typeof(ComparableObjectEqualityComparer<>)),
                () => CompareCollectionOfCollections(getCollectionTypes, typeof(ComparableStruct<>), typeof(ComparableStructEqualityComparer<>))
            );
        }

        private static void CompareCollectionOfCollections(
            Func<Type, Type[]> getCollectionTypes,
            Type genericContainer,
            Type genericSampleComparer)
        {
            CompareCollectionOfCollections(getCollectionTypes, false, false, genericContainer, genericSampleComparer);
            CompareCollectionOfCollections(getCollectionTypes, true, false, genericContainer, genericSampleComparer);
            CompareCollectionOfCollections(getCollectionTypes, false, true, genericContainer, genericSampleComparer);
            CompareCollectionOfCollections(getCollectionTypes, true, true, genericContainer, genericSampleComparer);
        }

        private static void CompareCollectionOfCollections(
            Func<Type, Type[]> getCollectionTypes,
            bool sort,
            bool nullable,
            Type genericSampleType,
            Type genericSampleComparer)
        {
            var types = nullable ? TestTypes.NullableTypes : TestTypes.Types;
            Parallel.ForEach(
                types,
                item => {
                    var (type, referenceComparer) = item;
                    var collections = getCollectionTypes(type);
                    var comparerTypes = collections
                        .Prepend(type)
                        .Take(collections.Length)
                        .Select(x => typeof(CollectionEqualityComparer<>).MakeGenericType(x))
                        .ToArray();
                    var comparer = comparerTypes.Aggregate(
                        referenceComparer,
                        (current, comparerType) => (IEqualityComparer)Activator.CreateInstance(comparerType, current, sort));

                    type = collections.Last();

                    if (genericSampleType != null) {
                        var comparerType = genericSampleComparer.MakeGenericType(type);
                        type = genericSampleType.MakeGenericType(type);
                        comparer = (IEqualityComparer)Activator.CreateInstance(comparerType, comparer);
                    }

                    new GenericTests().GenericTest(type, comparer, sort, Constants.SmallCount);
                });
        }
    }
}
