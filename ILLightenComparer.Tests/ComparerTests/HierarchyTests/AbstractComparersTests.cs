using System;
using System.Collections;
using AutoFixture;
using FluentAssertions;
using ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples.Nested;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests
{
    public sealed class AbstractComparersTests
    {
        [Fact]
        public void Comparison_Uses_Only_Members_Of_Abstract_Class()
        {
            var builder = new ComparerBuilder()
                .For<AbstractNestedObject>(c => c.SetDetectCycles(false));

            Test(builder);
        }

        [Fact]
        public void Comparison_Uses_Only_Members_Of_Base_Class()
        {
            var builder = new ComparerBuilder().For<BaseNestedObject>(
                c => c.SetDetectCycles(false)
                      .SetIgnoredMembers(new[] { nameof(BaseNestedObject.Key) }));

            Test(builder);
        }

        [Fact]
        public void Comparison_Uses_Only_Members_Of_Base_Interface()
        {
            var builder = new ComparerBuilder().For<INestedObject>(c => c.SetDetectCycles(false));

            Test(builder);
        }

        private void Test<T>(IComparerBuilder<T> builder)
        {
            var comparer = (IComparer)builder.GetComparer();

            for (var i = 0; i < 10; i++)
            {
                var x = _fixture.Create<SealedNestedObject>();
                var y = _fixture.Create<SealedNestedObject>();

                var expected = string.Compare(x.Text, y.Text, StringComparison.Ordinal).Normalize();
                var actual = comparer.Compare(x, y).Normalize();

                actual.Should().Be(expected, $"\nx: {x.ToJson()},\ny: {y.ToJson()}");
            }
        }

        private readonly Fixture _fixture = FixtureBuilder.GetInstance();
    }
}
