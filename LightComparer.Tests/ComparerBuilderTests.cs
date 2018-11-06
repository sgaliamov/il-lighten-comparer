using LightComparer.Tests.Samples;
using Xunit;

namespace LightComparer.Tests
{
    public class ComparerBuilderTests
    {
        public ComparerBuilderTests() => _builder = new ComparerBuilder(new Configuration());

        [Fact]
        public void Create_Generic_Comparer()
        {
            var comparer = _builder.CreateComparer<HaveNestedClass>();
        }

        [Fact]
        public void Create_Generic_EqualityComparer()
        {
            var comparer = _builder.CreateEqualityComparer<Simple>();
        }

        [Fact]
        public void Create_Not_Generic_Comparer()
        {
            var comparer = _builder.CreateComparer(typeof(Simple));
        }

        [Fact]
        public void Create_Not_Generic_EqualityComparer()
        {
            var comparer = _builder.CreateEqualityComparer(typeof(HaveNestedClass));
        }

        private readonly ComparerBuilder _builder;
    }
}
