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

            _builder = new ComparersBuilder().SetDefaultConfiguration(configuration);
        }

        [Fact]
        public void Create_Generic_Comparer()
        {
            var comparer = _builder.For<TestObject>().GetComparer();

            comparer.Should().NotBeNull();
        }

        [Fact(Skip = "Not implemented yet")]
        public void Create_Generic_EqualityComparer()
        {
            var comparer = _builder.GetEqualityComparer<TestObject>();

            comparer.Should().NotBeNull();
        }

        [Fact]
        public void Create_Not_Generic_Comparer()
        {
            var comparer = _builder.GetComparer(typeof(TestObject));

            comparer.Should().NotBeNull();
        }

        [Fact(Skip = "Not implemented yet")]
        public void Create_Not_Generic_EqualityComparer()
        {
            var comparer = _builder.GetEqualityComparer(typeof(TestObject));

            comparer.Should().NotBeNull();
        }

        [Fact]
        public void Generic_And_NotGeneric_Builders_Produce_The_Same_Comparer()
        {
            void Test(object one, object other)
            {
                one.GetType().Should().BeSameAs(other.GetType());
                other.Should().BeSameAs(one);
            }

            Test(_builder.GetComparer(typeof(TestObject)), _builder.GetComparer<TestObject>());
            Test(_builder.GetComparer<TestObject>(), _builder.GetComparer(typeof(TestObject)));
        }

        private readonly ComparersBuilder _builder;
    }
}
