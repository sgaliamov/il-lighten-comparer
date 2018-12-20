using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests.SimpleTypesTests
{
    public sealed class Tests
    {
        [Fact]
        public void Create_Comparer_For_EnumSmall()
        {
            Test<EnumSmall>();
        }

        [Fact]
        public void Create_Comparer_For_Short()
        {
            Test<short>();
        }

        [Fact]
        public void Create_Comparer_For_String()
        {
            Test<string>();
        }

        private void Test<T>()
        {
            var comparer = new ComparersBuilder().GetComparer<T>();

            for (var i = 0; i < 100; i++)
            {
                var x = _fixture.Create<T>();
                var y = _fixture.Create<T>();

                var expected = typeof(T) == typeof(string)
                                   ? string.CompareOrdinal(x as string, y as string)
                                   : Comparer<T>.Default.Compare(x, y).Normalize();
                var actual = comparer.Compare(x, y).Normalize();

                actual.Should().Be(expected.Normalize());
            }
        }

        private readonly Fixture _fixture = FixtureBuilder.GetInstance();
    }
}
