using AutoFixture;
using FluentAssertions;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Samples.Comparers;
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

        [Fact]
        public void Reset_ignored_members()
        {
            var x = _fixture.Create<SampleObject<int>>();
            var y = _fixture.Create<SampleObject<int>>();
            var expected = new SampleObjectComparer<int>().Compare(x, y);

            var builder = new ComparerBuilder();
            var comparer1 = builder.For<SampleObject<int>>(c => c.IgnoreMembers(
                                       nameof(SampleObject<int>.Field),
                                       nameof(SampleObject<int>.Property)))
                                   .GetComparer();
            var comparer2 = builder.Configure(c => c.IgnoreMembers(typeof(SampleObject<int>), null))
                                   .GetComparer<SampleObject<int>>();

            comparer1.Compare(x, y).Should().Be(0);
            comparer2.Compare(x, y).Should().Be(expected);
        }

        private readonly Fixture _fixture = new Fixture();
    }
}
