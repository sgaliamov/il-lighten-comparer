using System.Collections;
using AutoFixture;
using FluentAssertions;
using ILLightenComparer.Tests.Samples;
using Xunit;

namespace ILLightenComparer.Tests.Comparers
{
    public sealed class ComparerTests
    {
        [Fact]
        public void Comparison_Null_With_Object_Produces_Negative_Value()
        {
            var obj = _fixture.Create<FlatObject>();

            var actual = _target.Compare(null, obj);

            actual.Should().BeLessThan(0);
        }

        [Fact]
        public void Comparison_With_Itself_Produces_0()
        {
            var obj = _fixture.Create<FlatObject>();

            var actual = _target.Compare(obj, obj);

            actual.Should().Be(0);
        }

        [Fact]
        public void Comparison_With_Null_Produces_Positive_Value()
        {
            var obj = _fixture.Create<FlatObject>();

            var actual = _target.Compare(obj, null);

            actual.Should().BeGreaterThan(0);
        }

        private readonly IComparer _target = new ComparersBuilder().CreateComparer(typeof(FlatObject));
        private readonly Fixture _fixture = new Fixture();
    }
}
