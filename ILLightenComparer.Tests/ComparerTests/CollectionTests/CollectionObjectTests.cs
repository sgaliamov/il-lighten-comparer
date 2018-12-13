using System;
using System.Linq;
using AutoFixture;
using Force.DeepCloner;
using ILLightenComparer.Tests.ComparerTests.CollectionTests.Samples;
using ILLightenComparer.Tests.ComparerTests.ComparableTests.Samples;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests.CollectionTests
{
    public class CollectionObjectTests
    {
        [Fact]
        public void Compare_Array_Of_Bytes()
        {
            var comparer = _builder.For<ArrayObject<sbyte>>().GetComparer();

            var one = Create<sbyte>();
            var other = one.DeepClone();

            Array.Sort(one, ArrayObject<sbyte>.Comparer);
            Array.Sort(other, comparer);

            one.ShouldBeSameOrder(other);
        }

        [Fact]
        public void Compare_Array_Of_Longs()
        {
            var comparer = _builder.For<ArrayObject<long>>().GetComparer();

            var one = Create<long>();
            var other = one.DeepClone();

            Array.Sort(one, ArrayObject<long>.Comparer);
            Array.Sort(other, comparer);

            one.ShouldBeSameOrder(other);
        }

        [Fact]
        public void Compare_Array_Of_Comparable_Objects()
        {
            var comparer = _builder.For<ArrayObject<ComparableChildObject>>().GetComparer();

            var one = Create<ComparableChildObject>();
            var other = one.DeepClone();

            Array.Sort(one, ArrayObject<ComparableChildObject>.Comparer);
            Array.Sort(other, comparer);

            one.ShouldBeSameOrder(other);
        }

        private const int ItemsCount = 100;

        private ArrayObject<T>[] Create<T>() where T : IComparable<T>
        {
            var objects = new ArrayObject<T>[ItemsCount];
            for (var index = 0; index < objects.Length; index++)
            {
                objects[index] = new ArrayObject<T>
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
