using AutoFixture;
using FluentAssertions;
using ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests
{
    public class AbstractMembersTests
    {
        [Fact]
        public void AbstractProperty_Comparison()
        {
            var comparer = _contextBuilder.GetComparer<AbstractProperties>();

            for (var i = 0; i < 1000; i++)
            {
                var one = _fixture
                          .Build<AbstractProperties>()
                          .WithAutoProperties()
                          .With(x => x.AbstractProperty, _fixture.Create<NestedObject>())
                          .Create();

                var another = _fixture
                              .Build<AbstractProperties>()
                              .WithAutoProperties()
                              .With(x => x.AbstractProperty, _fixture.Create<NestedObject>())
                              .Create();

                var expected = AbstractProperties.AbstractPropertiesComparer.Compare(one, another);

                comparer.Compare(one, another).Should().Be(expected);
            }
        }

        private readonly Fixture _fixture = FixtureBuilder.GetInstance();

        private readonly IContextBuilder _contextBuilder =
            new ComparersBuilder().DefineDefaultConfiguration(new ComparerSettings
            {
                IncludeFields = true
            });
    }
}
