using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Force.DeepCloner;
using ILLightenComparer.Tests.ComparerTests.ComparableTests.Samples;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Samples.Comparers;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests.CollectionTests
{
    public sealed class EnumerableTests
    {
        [Fact]
        public void Compare_Enumerable_Of_Bytes()
        {
            CompareObjectEnumerableOf<byte>();

            CompareStructEnumerableOf<byte>();
        }

        [Fact]
        public void Compare_Enumerable_Of_Comparable_Nullable_Struct()
        {
            CompareObjectEnumerableOfNullable<ComparableStruct<EnumSmall>>();

            CompareStructEnumerableOfNullable<ComparableStruct<string>>();
        }

        [Fact]
        public void Compare_Enumerable_Of_Comparable_Objects()
        {
            CompareObjectEnumerableOf<ComparableObject>();
            CompareStructEnumerableOf<ComparableObject>();

            CompareObjectEnumerableOf<ComparableChildObject>();
            CompareStructEnumerableOf<ComparableChildObject>();
        }

        [Fact]
        public void Compare_Enumerable_Of_Comparable_Struct()
        {
            CompareObjectEnumerableOf<ComparableStruct<int>>();

            CompareStructEnumerableOf<ComparableStruct<decimal>>();
        }

        [Fact]
        public void Compare_Enumerable_Of_Enumerables()
        {
            Assert.Throws<NotSupportedException>(
                () => _builder.For<SampleObject<IEnumerable<IEnumerable<int>>>>().GetComparer());
            Assert.Throws<NotSupportedException>(
                () => _builder.For<SampleObject<IEnumerable<int[,]>>>().GetComparer());
            Assert.Throws<NotSupportedException>(
                () => _builder.For<SampleObject<IEnumerable<int[]>>>().GetComparer());
            Assert.Throws<NotSupportedException>(
                () => _builder.For<SampleObject<IEnumerable<int>[]>>().GetComparer());

            Assert.Throws<NotSupportedException>(
                () => _builder.For<SampleStruct<IEnumerable<IEnumerable<int>>>>().GetComparer());
            Assert.Throws<NotSupportedException>(
                () => _builder.For<SampleStruct<IEnumerable<int[,]>>>().GetComparer());
            Assert.Throws<NotSupportedException>(
                () => _builder.For<SampleStruct<IEnumerable<int[]>>>().GetComparer());
            Assert.Throws<NotSupportedException>(
                () => _builder.For<SampleStruct<IEnumerable<int>[]>>().GetComparer());
        }

        [Fact]
        public void Compare_Enumerable_Of_Enums()
        {
            CompareObjectEnumerableOf<EnumSmall>();

            CompareStructEnumerableOf<EnumSmall>();
        }

        [Fact]
        public void Compare_Enumerable_Of_Longs()
        {
            CompareObjectEnumerableOf<long>();

            CompareStructEnumerableOf<long>();
        }

        [Fact]
        public void Compare_Enumerable_Of_Nested_Objects()
        {
            var nestedComparer = new SampleObjectComparer<int>();
            var comparer = new SampleObjectComparer<SampleObject<int>>(nestedComparer);

            CompareObjectEnumerableOf(comparer);

            CompareStructEnumerableOf(comparer);
        }

        [Fact]
        public void Compare_Enumerable_Of_Nullable_Enums()
        {
            CompareObjectEnumerableOfNullable<EnumSmall>();

            CompareStructEnumerableOfNullable<EnumSmall>();
        }

        [Fact]
        public void Compare_Enumerable_Of_Nullable_Structs()
        {
            var comparer = new SampleStructComparer<int>();

            CompareObjectEnumerableOfNullable(comparer);

            CompareStructEnumerableOfNullable(comparer);
        }

        [Fact]
        public void Compare_Enumerable_Of_Strings()
        {
            CompareObjectEnumerableOf<string>();

            CompareStructEnumerableOf<string>();
        }

        [Fact]
        public void Compare_Enumerable_Of_Structs()
        {
            var comparer = new SampleStructComparer<int>();

            CompareObjectEnumerableOf(comparer);

            CompareStructEnumerableOf(comparer);
        }

        private void CompareObjectEnumerableOf<T>(IComparer<T> itemComparer = null)
        {
            var target = _builder.For<SampleObject<IEnumerable<T>>>().GetComparer();

            var one = CreateObjects<T>(ItemsCount).ToArray();
            var other = one.DeepClone();

            var collectionComparer = new CollectionComparer<IEnumerable<T>, T>(itemComparer);
            var referenceComparer = new SampleObjectComparer<IEnumerable<T>>(collectionComparer);

            Array.Sort(one, referenceComparer);
            Array.Sort(other, target);

            one.ShouldBeSameOrder(other);
        }

        private void CompareObjectEnumerableOfNullable<T>(IComparer<T> itemComparer = null) where T : struct
        {
            var nullableComparer = new NullableComparer<T>(itemComparer ?? Comparer<T>.Default);

            CompareObjectEnumerableOf(nullableComparer);
        }

        private void CompareStructEnumerableOf<T>(IComparer<T> itemComparer = null)
        {
            var target = _builder.For<SampleStruct<IEnumerable<T>>>().GetComparer();

            var one = CreateStructs<T>(ItemsCount).ToArray();
            var other = one.DeepClone();

            var collectionComparer = new CollectionComparer<IEnumerable<T>, T>(itemComparer);
            var referenceComparer = new SampleStructComparer<IEnumerable<T>>(collectionComparer);

            Array.Sort(one, referenceComparer);
            Array.Sort(other, target);

            one.ShouldBeSameOrder(other);
        }

        private void CompareStructEnumerableOfNullable<T>(IComparer<T> itemComparer = null) where T : struct
        {
            var nullableComparer = new NullableComparer<T>(itemComparer ?? Comparer<T>.Default);

            CompareStructEnumerableOf(nullableComparer);
        }

        private const int ItemsCount = 100;

        private IEnumerable<T> CreateEnumerable<T>()
        {
            if (_random.NextDouble() < 0.2)
            {
                return null;
            }

            var list = _fixture.CreateMany<T>(_random.Next(0, 5));

            if (_random.NextDouble() < 0.2)
            {
                list = list.Append(default);
            }

            return list.OrderBy(_ => _random.Next()).ToArray().AsEnumerable();
        }

        private IEnumerable<SampleObject<IEnumerable<T>>> CreateObjects<T>(int itemsCount)
        {
            for (var index = 0; index < itemsCount; index++)
            {
                yield return new SampleObject<IEnumerable<T>>
                {
                    Property = CreateEnumerable<T>(),
                    Field = CreateEnumerable<T>()
                };
            }
        }

        private IEnumerable<SampleStruct<IEnumerable<T>>> CreateStructs<T>(int itemsCount)
        {
            for (var index = 0; index < itemsCount; index++)
            {
                yield return new SampleStruct<IEnumerable<T>>
                {
                    Property = CreateEnumerable<T>(),
                    Field = CreateEnumerable<T>()
                };
            }
        }

        private readonly Fixture _fixture = FixtureBuilder.GetInstance();

        private readonly IContextBuilder _builder = new ComparersBuilder()
            .DefineDefaultConfiguration(new ComparerSettings { IncludeFields = true });

        private readonly Random _random = new Random();
    }
}
