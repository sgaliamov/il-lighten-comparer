using FluentAssertions;
using ILLightenComparer.Tests.Samples;
using Xunit;

namespace ILLightenComparer.Tests
{
    public sealed class ComparersBuilderTests
    {
        [Fact]
        public void Create_Generic_Comparer()
        {
            var comparer = _builder.For<DummyObject>().GetComparer();

            comparer.Should().NotBeNull();
        }

        [Fact(Skip = "Not implemented yet")]
        public void Create_Generic_EqualityComparer()
        {
            var comparer = _builder.For<DummyObject>().GetEqualityComparer();

            comparer.Should().NotBeNull();
        }

        private readonly IComparerBuilder _builder = new ComparerBuilder(config => config.DefaultIncludeFields(false));
    }
}
