using System;
using System.Linq;
using AutoFixture;
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
            var other = Create<sbyte>();

            Array.Sort(one, ArrayObject<sbyte>.Comparer);
            Array.Sort(other, comparer);

            one.ShouldBeSameOrder(other);
        }

        private ArrayObject<T>[] Create<T>() where T : IComparable<T>
        {
            var one = new ArrayObject<T>[100];
            foreach (var item in one)
            {
                item.ArrayProperty = _fixture.CreateMany<T>(_random.Next(5, 10)).ToArray();
            }

            return one;
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
