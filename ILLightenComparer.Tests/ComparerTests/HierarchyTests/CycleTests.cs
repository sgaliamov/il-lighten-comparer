using AutoFixture;
using FluentAssertions;
using Force.DeepCloner;
using ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples.Cycle;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests
{
    public class CycleTests
    {
        [Fact]
        public void Nested_Sealed_Should_Not_Fail()
        {
            var one = _fixture.Create<OneSealed>();

            var other = _fixture.Create<AnotherSealed>();

            //var comparer = _builder.GetComparer<SelfSealed>();

            //var expected = SelfSealed.Comparer.Compare(one, other);
            //var actual = comparer.Compare(one, other);

            //actual.Should().Be(expected);
        }

        [Fact]
        public void Self_Sealed_Should_Not_Fail()
        {
            var one = new SelfSealed();
            one.Self = new SelfSealed
            {
                Self = new SelfSealed
                {
                    Self = one
                }
            };
            var other = _fixture.Create<SelfSealed>();
            other.Self = one;

            SelfSealed.Comparer.Compare(one, one.DeepClone()).Should().Be(0);

            var expected = SelfSealed.Comparer.Compare(one, other);

            var comparer = _builder
                           .For<SelfSealed>()
                           .GetComparer();

            var actual = comparer.Compare(one, other);

            actual.Should().Be(expected);
        }

        private readonly Fixture _fixture = FixtureBuilder.GetInstance();

        private readonly IContextBuilder _builder =
            new ComparersBuilder()
                .DefineDefaultConfiguration(new ComparerSettings
                {
                    IncludeFields = true,
                    DetectCycles = true
                });
    }
}
