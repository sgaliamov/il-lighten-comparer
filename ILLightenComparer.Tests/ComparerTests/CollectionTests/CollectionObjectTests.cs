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
    public sealed class CollectionObjectTests
    {
        [Fact]
        public void Compare_Array_Of_Arrays()
        {
            Assert.Throws<NotSupportedException>(() => _builder.For<ArrayOfObjects<int[]>>().GetComparer());
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
        public void Compare_Array_Of_NullableEnums()
        {
            CompareArrayOf<EnumSmall?>();
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
        public void Compare_Array_Of_Strings()
        {
            CompareArrayOf<string>();
        }

        private void CompareArrayOf<T>(IComparer<T> itemComparer = null)
        {
            var comparer = _builder.For<ArrayOfObjects<T>>().GetComparer();

            var one = Create<T>(ItemsCount).ToArray();
            var other = one.DeepClone();

            Array.Sort(one, ArrayOfObjects<T>.GetComparer(itemComparer ?? Comparer<T>.Default));
            Array.Sort(other, comparer);

            one.ShouldBeSameOrder(other);
        }

        private const int ItemsCount = 100;

        private IEnumerable<ArrayOfObjects<T>> Create<T>(int itemsCount)
        {
            for (var index = 0; index < itemsCount; index++)
            {
                var array = _random.NextDouble() < 0.2
                                ? null
                                : _fixture.CreateMany<T>(_random.Next(0, 5)).ToArray();

                yield return new ArrayOfObjects<T>
                {
                    ArrayProperty = array
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
