using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Force.DeepCloner;
using ILLightenComparer.Tests.ComparerTests.CollectionTests.Samples;
using ILLightenComparer.Tests.ComparerTests.ComparableTests.Samples;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests.CollectionTests
{
    public sealed class ClassArrayTests
    {
        [Fact]
        public void Compare_Array_Of_Arrays()
        {
            Assert.Throws<NotSupportedException>(() => _builder.For<SampleObject<int[]>>().GetComparer());
        }

        [Fact]
        public void Compare_Array_Of_Bytes()
        {
            CompareArrayOf<byte>();
        }

        [Fact]
        public void Compare_Array_Of_Comparable_Objects()
        {
            CompareArrayOf<ComparableChildObject>();
        }

        [Fact]
        public void Compare_Array_Of_Enums()
        {
            CompareArrayOf<EnumSmall>();
        }

        [Fact]
        public void Compare_Array_Of_HierarchicalObjects()
        {
            CompareArrayOf(HierarchicalObject.Comparer);
        }

        [Fact]
        public void Compare_Array_Of_Longs()
        {
            CompareArrayOf<long>();
        }

        [Fact]
        public void Compare_Array_Of_NullableEnums()
        {
            CompareArrayOfNullable<EnumSmall>();
        }

        [Fact]
        public void Compare_Array_Of_Strings()
        {
            CompareArrayOf<string>();
        }

        private void CompareArrayOf<T>(IComparer<T> itemComparer = null)
        {
            var comparer = _builder.For<SampleObject<T[]>>().GetComparer();

            var one = Create<T>(ItemsCount).ToArray();
            var other = one.DeepClone();

            var collectionComparer = new CollectionComparer<T[], T>(itemComparer ?? Comparer<T>.Default);
            var comparison = SampleObject<T[]>.CreateComparer(collectionComparer);

            Array.Sort(one, comparison);
            Array.Sort(other, comparer);

            one.ShouldBeSameOrder(other);
        }

        private void CompareArrayOfNullable<T>(IComparer<T> itemComparer = null) where T : struct
        {
            var nullableComparer = new NullableComparer<T>(itemComparer ?? Comparer<T>.Default);

            CompareArrayOf(nullableComparer);
        }

        private const int ItemsCount = 100;

        private IEnumerable<SampleObject<T[]>> Create<T>(int itemsCount)
        {
            T[] Create()
            {
                return _random.NextDouble() < 0.2
                           ? null
                           : _fixture.CreateMany<T>(_random.Next(0, 5)).ToArray();
            }

            for (var index = 0; index < itemsCount; index++)
            {
                yield return new SampleObject<T[]>
                {
                    Property = Create(),
                    Field = Create()
                };
            }
        }

        private readonly Fixture _fixture = FixtureBuilder.GetInstance();

        private readonly IContextBuilder _builder = new ComparersBuilder()
            .DefineDefaultConfiguration(new ComparerSettings
            {
                IncludeFields = true,
                DetectCycles = false
            });

        private readonly Random _random = new Random();
    }
}
