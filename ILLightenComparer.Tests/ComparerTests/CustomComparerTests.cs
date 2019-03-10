using AutoFixture;
using FluentAssertions;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Samples.Comparers;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests
{
    public sealed class CustomComparerTests
    {
        [Fact]
        public void Custom_instance_comparer_for_primitive_member_should_be_used()
        {
            var fixture = FixtureBuilder.GetInstance();

            var comparer = new ComparerBuilder()
                           .For<int>(c => c.SetComparer(new CustomisableComparer<int>((x, y) => 0)))
                           .For<SampleObject<int>>()
                           .GetComparer();

            var one = fixture.Create<SampleObject<int>>();
            var other = fixture.Create<SampleObject<int>>();

            comparer.Compare(one, other).Should().Be(0);
        }
    }
}
