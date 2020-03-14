using System;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace ILLightenComparer.Tests
{
    public sealed class EqualityExamples
    {
        [Fact]
        public void Basic_usage()
        {
            var x = _fixture.Create<Tuple<int, string>>();
            var y = _fixture.Create<Tuple<int, string>>();

            var comparer = new ComparerBuilder().GetEqualityComparer<Tuple<int, string>>();

            var result = comparer.Equals(x, y);

            result.Should().BeFalse();
        }

        [Fact]
        public void Ignore_specific_members()
        {
            var x = new Tuple<int, string, double>(1, "value 1", 1.1);
            var y = new Tuple<int, string, double>(1, "value 2", 2.2);

            var comparer = new ComparerBuilder()
                           .For<Tuple<int, string, double>>()
                           .Configure(c => c.IgnoreMember(o => o.Item2)
                                            .IgnoreMember(o => o.Item3))
                           .GetEqualityComparer();

            var result = comparer.Equals(x, y);

            result.Should().BeTrue();

            comparer.GetHashCode(x).Should().Be(comparer.GetHashCode(y));
        }

        private readonly Fixture _fixture = new Fixture();
    }
}
