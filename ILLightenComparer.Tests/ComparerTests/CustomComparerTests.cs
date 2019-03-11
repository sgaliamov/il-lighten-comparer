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
            var comparer = new ComparerBuilder()
                           .For<int>(c => c.SetComparer(new CustomisableComparer<int>((x, y) => 0)))
                           .GetComparer<SampleObject<int>>();

            var one = _fixture.Create<SampleObject<int>>();
            var other = new SampleObject<int>
            {
                Field = one.Field + 1,
                Property = one.Property - 1
            };

            comparer.Compare(one, other).Should().Be(0);
        }

        private readonly Fixture _fixture = FixtureBuilder.GetInstance();
    }
}
