using FluentAssertions;
using ILLightenComparer.Tests.Samples;
using Xunit;

namespace ILLightenComparer.Tests
{
    public class ComparersBuilderTests
    {
        public ComparersBuilderTests()
        {
            var configuration = new CompareConfiguration
            {
                IncludeFields = false
            };

            _builder = new ComparersBuilder().SetConfiguration(configuration);
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
            var comparer = _builder.CreateEqualityComparer<TestObject>();

            comparer.Should().NotBeNull();
        }

        [Fact]
        public void Create_Not_Generic_Comparer()
        {
            var comparer = _builder.CreateComparer(typeof(TestObject));

            comparer.Should().NotBeNull();
        }

        [Fact]
        public void Create_Not_Generic_EqualityComparer()
        {
            var comparer = _builder.CreateEqualityComparer(typeof(HierarchicalObject));

            comparer.Should().NotBeNull();
        }

        private readonly IComparersBuilder _builder;
    }
}
