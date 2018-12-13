using System;
using System.Linq;
using AutoFixture;
using Force.DeepCloner;
using ILLightenComparer.Tests.ComparerTests.CollectionTests.Samples;
using ILLightenComparer.Tests.ComparerTests.ComparableTests.Samples;
using ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests.CollectionTests
{
    public class CollectionObjectTests
    {
        [Fact]
        public void Compare_Array_Of_Arrays()
        {
            Assert.Throws<NotSupportedException>(() => _builder.For<ArrayOfObjects<int[]>>().GetComparer());
        }

        [Fact]
        public void Compare_Array_Of_Bytes()
        {
            var comparer = _builder.For<ArrayOfObjects<sbyte>>().GetComparer();

            var one = Create<sbyte>();
            var other = one.DeepClone();

            Array.Sort(one, ArrayOfObjects<sbyte>.Comparer);
            Array.Sort(other, comparer);

            one.ShouldBeSameOrder(other);
        }

        [Fact]
        public void Compare_Array_Of_Comparable_Objects()
        {
            var comparer = _builder.For<ArrayOfObjects<ComparableChildObject>>().GetComparer();

            var one = Create<ComparableChildObject>();
            var other = one.DeepClone();

            Array.Sort(one, ArrayOfObjects<ComparableChildObject>.Comparer);
            Array.Sort(other, comparer);

            one.ShouldBeSameOrder(other);
        }

        [Fact]
        public void Compare_Array_Of_HierarchicalObjects()
        {
            var comparer = _builder.For<ArrayOfObjects<HierarchicalObject>>().GetComparer();

            var one = Create<HierarchicalObject>();
            var other = one.DeepClone();

            Array.Sort(other, comparer);
        }

        [Fact]
        public void Compare_Array_Of_Longs()
        {
            var comparer = _builder.For<ArrayOfObjects<long>>().GetComparer();

            var one = Create<long>();
            var other = one.DeepClone();

            Array.Sort(one, ArrayOfObjects<long>.Comparer);
            Array.Sort(other, comparer);

            one.ShouldBeSameOrder(other);
        }

        [Fact]
        public void Compare_Array_Of_Strings()
        {
            var comparer = _builder.For<ArrayOfObjects<string>>().GetComparer();

            var one = Create<string>();
            var other = one.DeepClone();

            Array.Sort(one, ArrayOfObjects<string>.Comparer);
            Array.Sort(other, comparer);

            one.ShouldBeSameOrder(other);
        }

        private const int ItemsCount = 100;

        private ArrayOfObjects<T>[] Create<T>()
        {
            var objects = new ArrayOfObjects<T>[ItemsCount];
            for (var index = 0; index < objects.Length; index++)
            {
                objects[index] = new ArrayOfObjects<T>
                {
                    ArrayProperty = _fixture.CreateMany<T>(_random.Next(0, 5)).ToArray()
                };
            }

            return objects;
        }

        private readonly Fixture _fixture = FixtureBuilder.GetInstance();

        private readonly IContextBuilder _builder = new ComparersBuilder()
            .DefineDefaultConfiguration(new ComparerSettings
            {
                IncludeFields = true
            });

        private readonly Random _random = new Random();
    }
}
