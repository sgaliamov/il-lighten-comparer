using System;
using System.Linq;
using AutoFixture;
using Force.DeepCloner;
using ILLightenComparer.Tests.ComparerTests.CollectionTests.Samples;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests.CollectionTests
{
    public class CollectionObjectTests
    {
        [Fact]
        public void Compare_Array()
        {
            var comparer = _builder.For<ArrayObject<sbyte>>().GetComparer();

            var one = Create<sbyte>();
            var other = one.DeepClone();

            Array.Sort(one, ArrayObject<sbyte>.Comparer);
            Array.Sort(other, comparer);

            one.ShouldBeSameOrder(other);
        }

        [Fact]
        public void Compare_Nullable_Array()
        {
            //var comparer = _builder.For<ArrayObject<int?>>().GetComparer();

            //var one = Create<sbyte>();
            //var other = Create<sbyte>();

            //Array.Sort(one, ArrayObject<sbyte>.Comparer);
            //Array.Sort(other, comparer);

            //one.ShouldBeSameOrder(other);
        }

        private ArrayObject<T>[] Create<T>() where T : IComparable<T>
        {
            var objects = new ArrayObject<T>[100];
            for (var index = 0; index < objects.Length; index++)
            {
                objects[index] = new ArrayObject<T>
                {
                    ArrayProperty = _fixture.CreateMany<T>(_random.Next(5, 10)).ToArray()
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
