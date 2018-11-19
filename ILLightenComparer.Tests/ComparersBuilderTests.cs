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
            var comparer = _builder.CreateComparer<TestObject>();

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
            var comparer = _builder.CreateEqualityComparer(typeof(TestObject));

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

            Test(_builder.CreateComparer(typeof(TestObject)), _builder.CreateComparer<TestObject>());
            Test(_builder.CreateComparer<SampleStruct>(), _builder.CreateComparer(typeof(SampleStruct)));
        }

        private readonly IComparersBuilder _builder;
    }
}
