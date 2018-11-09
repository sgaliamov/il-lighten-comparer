using FluentAssertions;
using ILLightenComparer.Tests.Samples;
using Xunit;

namespace ILLightenComparer.Tests
{
    public class ComparerBuilderTests
    {
        public ComparerBuilderTests()
        {
            var configuration = new CompareConfiguration
            {
                IncludeFields = false
            };

            _builder = new ComparerBuilder().SetConfiguration(configuration);
        }

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

        private readonly IComparerBuilder _builder;
    }
}
