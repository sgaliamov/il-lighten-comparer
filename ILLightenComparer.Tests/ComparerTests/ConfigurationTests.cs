using AutoFixture;
using FluentAssertions;
using ILLightenComparer.Tests.Samples;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests
{
    public sealed class ConfigurationTests
    {
        [Fact]
        public void Ignore_all_members_using_expressions()
        {
            var x = _fixture.Create<SampleObject<int>>();
            var y = _fixture.Create<SampleObject<int>>();

            var comparer = new ComparerBuilder()
                           .For<SampleObject<int>>(c => c.IgnoreMember(o => o.Field)
                                                         .IgnoreMember(o => o.Property))
                           .GetComparer();

            comparer.Compare(x, y).Should().Be(0);
        }

        private readonly Fixture _fixture = new Fixture();
    }
}
