using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Force.DeepCloner;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Samples.Comparers;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests.CollectionTests
{
    public sealed class ArrayTests
    {
        [Fact]
        public void Compare_Array_Of_Arrays()
        {
            var builder = new ComparersBuilder()
                .DefineDefaultConfiguration(new ComparerSettings { IncludeFields = true });

            Assert.Throws<NotSupportedException>(() => builder.For<SampleObject<int[][]>>().GetComparer());
            Assert.Throws<NotSupportedException>(() => builder.For<SampleObject<int[][,]>>().GetComparer());
            Assert.Throws<NotSupportedException>(() => builder.For<SampleObject<int[,]>>().GetComparer());

            Assert.Throws<NotSupportedException>(() => builder.For<SampleStruct<int[][]>>().GetComparer());
            Assert.Throws<NotSupportedException>(() => builder.For<SampleStruct<int[][,]>>().GetComparer());
            Assert.Throws<NotSupportedException>(() => builder.For<SampleStruct<int[,]>>().GetComparer());
        }

        [Fact]
        public void Compare_Array_Of_Bytes()
        {
            CompareObjectArrayOf<byte>();
            CompareStructArrayOf<byte>();

            CompareObjectArrayOf<byte>(null, true);
            CompareStructArrayOf<byte>(null, true);
        }

        [Fact]
        public void Compare_Array_Of_Comparable_Nullable_Struct()
        {
            CompareObjectArrayOfNullable<SampleComparableStruct<EnumSmall>>();
            CompareStructArrayOfNullable<SampleComparableStruct<string>>();

            CompareObjectArrayOfNullable<SampleComparableStruct<EnumSmall>>(null, true);
            CompareStructArrayOfNullable<SampleComparableStruct<string>>(null, true);
        }

        [Fact]
        public void Compare_Array_Of_Comparable_Objects()
        {
            CompareObjectArrayOf<SampleComparableObject>();
            CompareStructArrayOf<SampleComparableObject>();

            CompareObjectArrayOf<SampleComparableChildObject>();
            CompareStructArrayOf<SampleComparableChildObject>();

            CompareObjectArrayOf<SampleComparableObject>(null, true);
            CompareStructArrayOf<SampleComparableObject>(null, true);

            CompareObjectArrayOf<SampleComparableChildObject>(null, true);
            CompareStructArrayOf<SampleComparableChildObject>(null, true);
        }

        [Fact]
        public void Compare_Array_Of_Comparable_Struct()
        {
            CompareObjectArrayOf<SampleComparableStruct<int>>();
            CompareStructArrayOf<SampleComparableStruct<decimal>>();

            CompareObjectArrayOf<SampleComparableStruct<int>>(null, true);
            CompareStructArrayOf<SampleComparableStruct<decimal>>(null, true);
        }

        [Fact]
        public void Compare_Array_Of_Enums()
        {
            CompareObjectArrayOf<EnumSmall>();
            CompareStructArrayOf<EnumSmall>();

            CompareObjectArrayOf<EnumSmall>(null, true);
            CompareStructArrayOf<EnumSmall>(null, true);
        }

        [Fact]
        public void Compare_Array_Of_Longs()
        {
            CompareObjectArrayOf<long>();
            CompareStructArrayOf<ulong>();

            CompareObjectArrayOf<ulong>(null, true);
            CompareStructArrayOf<long>(null, true);
        }

        [Fact]
        public void Compare_Array_Of_Nested_Objects()
        {
            var nestedComparer = new SampleObjectComparer<int>();
            var comparer = new SampleObjectComparer<SampleObject<int>>(nestedComparer);

            CompareObjectArrayOf(comparer);
            CompareStructArrayOf(comparer);

            CompareObjectArrayOf(comparer, true);
            CompareStructArrayOf(comparer, true);
        }

        [Fact]
        public void Compare_Array_Of_Nullable_Enums()
        {
            CompareObjectArrayOfNullable<EnumSmall>();
            CompareStructArrayOfNullable<EnumSmall>();

            CompareObjectArrayOfNullable<EnumSmall>(null, true);
            CompareStructArrayOfNullable<EnumSmall>(null, true);
        }

        [Fact]
        public void Compare_Array_Of_Nullable_Structs()
        {
            var comparer = new SampleStructComparer<int>();

            CompareObjectArrayOfNullable(comparer);
            CompareStructArrayOfNullable(comparer);

            CompareObjectArrayOfNullable(comparer, true);
            CompareStructArrayOfNullable(comparer, true);
        }

        [Fact]
        public void Compare_Array_Of_Strings()
        {
            CompareObjectArrayOf<string>();
            CompareStructArrayOf<string>();

            CompareObjectArrayOf<string>(null, true);
            CompareStructArrayOf<string>(null, true);
        }

        [Fact]
        public void Compare_Array_Of_Structs()
        {
            var comparer = new SampleStructComparer<int?>();

            CompareObjectArrayOf(comparer);
            CompareStructArrayOf(comparer);

            CompareObjectArrayOf(comparer, true);
            CompareStructArrayOf(comparer, true);
        }

        private void CompareObjectArrayOf<T>(IComparer<T> itemComparer = null, bool sort = false)
        {
            var target = new ComparersBuilder()
                         .DefineDefaultConfiguration(new ComparerSettings { IncludeFields = true })
                         .For<SampleObject<T[]>>()
                         .DefineConfiguration(new ComparerSettings { IgnoreCollectionOrder = sort })
                         .GetComparer();

            var one = CreateObjects<T>(ItemsCount).OrderBy(_ => _random.Next()).ToArray();
            var other = one.DeepClone();

            var collectionComparer = new CollectionComparer<T[], T>(itemComparer, sort);
            var referenceComparer = new SampleObjectComparer<T[]>(collectionComparer);

            Array.Sort(one, referenceComparer);
            Array.Sort(other, target);

            one.ShouldBeSameOrder(other);
        }

        private void CompareStructArrayOf<T>(IComparer<T> itemComparer = null, bool sort = false)
        {
            var target = new ComparersBuilder()
                         .DefineDefaultConfiguration(new ComparerSettings { IncludeFields = true })
                         .For<SampleStruct<T[]>>()
                         .DefineConfiguration(new ComparerSettings { IgnoreCollectionOrder = sort })
                         .GetComparer();

            var one = CreateStructs<T>(ItemsCount).ToArray();
            var other = one.DeepClone();

            var collectionComparer = new CollectionComparer<T[], T>(itemComparer, sort);
            var referenceComparer = new SampleStructComparer<T[]>(collectionComparer);

            Array.Sort(one, referenceComparer);
            Array.Sort(other, target);

            one.ShouldBeSameOrder(other);
        }

        private void CompareObjectArrayOfNullable<T>(
            IComparer<T> itemComparer = null,
            bool sort = false) where T : struct
        {
            var nullableComparer = new NullableComparer<T>(itemComparer ?? Comparer<T>.Default);

            CompareObjectArrayOf(nullableComparer, sort);
        }

        private void CompareStructArrayOfNullable<T>(IComparer<T> itemComparer = null, bool sort = false) where T : struct
        {
            var nullableComparer = new NullableComparer<T>(itemComparer ?? Comparer<T>.Default);

            CompareStructArrayOf(nullableComparer, sort);
        }

        private const int ItemsCount = 100;

        private T[] CreateArray<T>()
        {
            if (_random.NextDouble() < 0.2)
            {
                return null;
            }

            var list = _fixture.CreateMany<T>(_random.Next(0, 5)).ToList();

            if (_random.NextDouble() < 0.2)
            {
                list.Add(default);
                list.Add(default);
            }

            return list.OrderBy(_ => _random.Next()).ToArray();
        }

        private IEnumerable<SampleObject<T[]>> CreateObjects<T>(int itemsCount)
        {
            for (var i = 0; i < itemsCount / 10; i++)
            {
                yield return null;
            }

            for (var index = 0; index < itemsCount; index++)
            {
                yield return new SampleObject<T[]>
                {
                    Property = CreateArray<T>(),
                    Field = CreateArray<T>()
                };
            }
        }

        private IEnumerable<SampleStruct<T[]>> CreateStructs<T>(int itemsCount)
        {
            for (var index = 0; index < itemsCount; index++)
            {
                yield return new SampleStruct<T[]>
                {
                    Property = CreateArray<T>(),
                    Field = CreateArray<T>()
                };
            }
        }

        private readonly Fixture _fixture = FixtureBuilder.GetInstance();

        private readonly Random _random = new Random();
    }
}
