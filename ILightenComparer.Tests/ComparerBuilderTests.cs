using FluentAssertions;
using ILightenComparer.Tests.Samples;
using Xunit;

namespace ILightenComparer.Tests
{
    public class ComparerBuilderTests
    {
        public ComparerBuilderTests() => _builder = new ComparerBuilder(new CompareConfiguration());

        [Fact]
        public void Create_Generic_Comparer()
        {
            var comparer = _builder.CreateComparer<HierarchicalObject>();

            comparer.Should().NotBeNull();
        }

        [Fact]
        public void Create_Generic_EqualityComparer()
        {
            var comparer = _builder.CreateEqualityComparer<FlatObject>();

            comparer.Should().NotBeNull();
        }

        [Fact]
        public void Create_Not_Generic_Comparer()
        {
            var comparer = _builder.CreateComparer(typeof(FlatObject));

            comparer.Should().NotBeNull();
        }

        [Fact]
        public void Create_Not_Generic_EqualityComparer()
        {
            var comparer = _builder.CreateEqualityComparer(typeof(HierarchicalObject));

            comparer.Should().NotBeNull();
        }

        private readonly ComparerBuilder _builder;
    }
}
