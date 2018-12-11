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

            var one = Create();
            var other = Create();

            Array.Sort(one, ArrayObject<sbyte>.Comparer);
            Array.Sort(other, comparer);

            one.ShouldBeSameOrder(other);
        }

        private ArrayObject<sbyte>[] Create()
        {
            var one = new ArrayObject<sbyte>[100];
            foreach (var item in one)
            {
                item.ArrayProperty = _fixture.CreateMany<sbyte>(_random.Next(10, 20)).ToArray();
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
