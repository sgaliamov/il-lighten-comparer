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

        [Fact]
        public void Create_Not_Generic_Comparer()
        {
            var comparer = _builder.GetComparer(typeof(DummyObject));

            comparer.Should().NotBeNull();
        }

        [Fact(Skip = "Not implemented yet")]
        public void Create_Not_Generic_EqualityComparer()
        {
            var comparer = _builder.GetEqualityComparer(typeof(DummyObject));

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

            Test(_builder.GetComparer(typeof(DummyObject)), _builder.GetComparer<DummyObject>());
            Test(_builder.GetComparer<DummyObject>(), _builder.GetComparer(typeof(DummyObject)));
        }

        private readonly IContextBuilder _builder = new ComparersBuilder()
            .DefineDefaultConfiguration(new ComparerSettings { IncludeFields = false });
    }
}
