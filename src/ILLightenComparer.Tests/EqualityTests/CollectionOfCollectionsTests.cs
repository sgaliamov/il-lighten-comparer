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
using Illuminator.Extensions;
using Xunit;

namespace ILLightenComparer.Tests.EqualityTests
{
    public sealed class CollectionOfCollectionsTests
    {
        [Fact]
        public void Compare_enumerables_of_enumerables()
        {
            static int GetHashCode(IEnumerable<EnumerableStruct<List<int?>>?> input)
            {
                var num = HashCodeCombiner.Seed;
                if (input == null) {
                    return 0;
                }

                var enumerator = input.GetEnumerator();
                while (enumerator.MoveNext()) {
                    var num2 = (num << 5) + num;
                    var enumerableStruct = enumerator.Current;
                    long num3;
                    if (enumerableStruct == null) {
                        num3 = 0L;
                    } else {
                        var num4 = HashCodeCombiner.Seed;
                        var enumerator2 = enumerableStruct.Value.GetEnumerator();
                        if (enumerator2 == null) {
                            num3 = 0L;
                        } else {
                            while (enumerator2.MoveNext()) {
                                var num5 = (num4 << 5) + num4;
                                var list = enumerator2.Current;
                                long num6;
                                if (list == null) {
                                    num6 = 0L;
                                } else {
                                    var enumerator3 = list.GetEnumerator();
                                    while (enumerator3.MoveNext()) {
                                        var num7 = (num4 << 5) + num4;
                                        var num8 = enumerator3.Current;
                                        num4 = (num7 ^ (num8?.GetHashCode() ?? 0));
                                    }
                                    num6 = num4;
                                }
                                num4 = (num5 ^ num6);
                            }
                            num3 = num4;
                        }
                    }
                    num = (num2 ^ num3);
                }

                return (int)num;
            }

            var comparer = new ComparerBuilder().GetEqualityComparer<IEnumerable<EnumerableStruct<List<int?>>?>>();
            var listComparer = new CollectionEqualityComparer<List<int?>>(new CollectionEqualityComparer<int?>());
            var customizableComparer = new CustomizableEqualityComparer<EnumerableStruct<List<int?>>>(
                (a, b) => listComparer.Equals(a, b),
                _ => throw new NotSupportedException());
            var collectionComparer = new CollectionEqualityComparer<EnumerableStruct<List<int?>>?>(new NullableEqualityComparer<EnumerableStruct<List<int?>>>(customizableComparer));
            var referenceComparer = new CustomizableEqualityComparer<IEnumerable<EnumerableStruct<List<int?>>?>>(
                (a, b) => collectionComparer.Equals(a, b),
                GetHashCode);

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
            });
        }

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

        private readonly IFixture _fixture = FixtureBuilder.GetInstance();

        private static void CompareCollectionOfCollections(Func<Type, Type[]> getCollectionTypes)
        {
            Parallel.Invoke(
                () => CompareCollectionOfCollections(getCollectionTypes, null, null),
                () => CompareCollectionOfCollections(getCollectionTypes, typeof(ComparableObject<>), typeof(ComparableObjectEqualityComparer<>)),
                () => CompareCollectionOfCollections(getCollectionTypes, typeof(ComparableStruct<>), typeof(ComparableStructEqualityComparer<>))
            );
        }

        private static void CompareCollectionOfCollections(Func<Type, Type[]> getCollectionTypes, Type genericContainer, Type genericSampleComparer)
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

            var hashMethod = typeof(CollectionOfCollectionsTests)
                .GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                .Where(x => x.Name == nameof(GetTwoLevelHashCode))
                .Single(x => {
                    var type = x.GetParameters()[0].ParameterType;
                    if (genericSampleType == null) {
                        return type.ImplementsGeneric(typeof(IEnumerable<>));
                    }

                    return type.Name == genericSampleType.Name;
                });

            Parallel.ForEach(
                types,
                item => {
                    var (itemType, itemComparer) = item;
                    var collections = getCollectionTypes(itemType);
                    var comparer = collections
                        .Prepend(itemType)
                        .Take(collections.Length)
                        .Select(x => typeof(CollectionEqualityComparer<>).MakeGenericType(x))
                        .Aggregate(itemComparer, (current, comparerType) => (IEqualityComparer)Activator.CreateInstance(comparerType, current, sort));

                    var combinedType = collections.Last();

                    if (genericSampleType != null) {
                        var comparerType = genericSampleComparer.MakeGenericType(combinedType);
                        combinedType = genericSampleType.MakeGenericType(combinedType);
                        comparer = (IEqualityComparer)Activator.CreateInstance(comparerType, comparer);
                    }

                    var customizableComparerType = typeof(UglyEqualityComparer<>).MakeGenericType(combinedType);

                    var customizableComparer = (IEqualityComparer)Activator.CreateInstance(customizableComparerType, comparer, hashMethod.MakeGenericMethod(itemType));

                    new GenericTests().GenericTest(combinedType, customizableComparer, sort, 1);
                });
        }

        private static int GetTwoLevelHashCode<T>(ComparableStruct<IEnumerable<IEnumerable<T>>> obj) => GetTwoLevelHashCode(new ComparableObject<IEnumerable<IEnumerable<T>>> {
            Field = obj.Field,
            Property = obj.Property
        });

        private static int GetTwoLevelHashCode<T>(IEnumerable<IEnumerable<T>> collection)
        {
            var num = HashCodeCombiner.Seed;
            if (collection == null) {
                return 0;
            }

            var enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext()) {
                var num2 = (num << 5) + num;
                var list = enumerator.Current;
                long num3;
                if (list == null) {
                    num3 = 0L;
                } else {
                    var enumerator2 = list.GetEnumerator();
                    while (enumerator2.MoveNext()) {
                        var num4 = (num << 5) + num;
                        var b = enumerator2.Current;
                        num = (num4 ^ b.GetHashCode());
                    }
                    num3 = num;
                }
                num = (num2 ^ num3);
            }

            return (int)num;
        }

        private static int GetTwoLevelHashCode<T>(ComparableObject<IEnumerable<IEnumerable<T>>> obj)
        {
            if (obj == null) {
                return 0;
            }

            var num = HashCodeCombiner.Seed;
            var num2 = (num << 5) + num;
            var list = obj.Field;
            long num3;

            if (list == null) {
                num3 = 0L;
            } else {
                var enumerator = list.GetEnumerator();
                while (enumerator.MoveNext()) {
                    var num4 = (num << 5) + num;
                    var list2 = enumerator.Current;
                    long num5;
                    if (list2 == null) {
                        num5 = 0L;
                    } else {
                        var enumerator2 = list2.GetEnumerator();
                        while (enumerator2.MoveNext()) {
                            var num6 = (num << 5) + num;
                            var b = enumerator2.Current;
                            num = (num6 ^ b.GetHashCode());
                        }
                        num5 = num;
                    }
                    num = (num4 ^ num5);
                }
                num3 = num;
            }
            num = (num2 ^ num3);
            var num7 = (num << 5) + num;
            list = obj.Property;
            long num8;
            if (list == null) {
                num8 = 0L;
            } else {
                var enumerator = list.GetEnumerator();
                while (enumerator.MoveNext()) {
                    var num9 = (num << 5) + num;
                    var list2 = enumerator.Current;
                    long num10;
                    if (list2 == null) {
                        num10 = 0L;
                    } else {
                        var enumerator2 = list2.GetEnumerator();
                        while (enumerator2.MoveNext()) {
                            var num11 = (num << 5) + num;
                            var b = enumerator2.Current;
                            num = (num11 ^ b.GetHashCode());
                        }
                        num10 = num;
                    }
                    num = (num9 ^ num10);
                }
                num8 = num;
            }
            num = (num7 ^ num8);

            return (int)num;
        }
    }
}
