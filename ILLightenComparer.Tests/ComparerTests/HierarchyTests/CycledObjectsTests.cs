using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Force.DeepCloner;
using ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples.Cycled;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests
{
    public class CycledObjectsTests
    {
        public CycledObjectsTests()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var builder = new ComparersBuilder()
                .DefineDefaultConfiguration(new ComparerSettings
                {
                    IncludeFields = true,
                    DetectCycles = true
                });

            _comparerOneSealed = builder.For<OneSealed>().GetComparer();
            _comparerSelfOpened = builder.For<SelfOpened>().GetComparer();
            _comparerSelfSealed = builder.For<SelfSealed>()
                                         .DefineConfiguration(new ComparerSettings
                                         {
                                             IgnoredMembers = new[] { nameof(SelfSealed.Id) }
                                         })
                                         .GetComparer();
        }

        [Fact]
        public void Comparison_Should_Not_Fail_Because_Of_Generating_Comparers_For_Two_Dependent_Classes()
        {
            var one = _fixture.Create<OneSealed>();
            var other = _fixture.Create<OneSealed>();
            one.Another.One = one;
            other.Another.One = other;

            var expected = one.Value.CompareTo(other.Value);
            var actual = _comparerOneSealed.Compare(one, other);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Comparison_With_Cycle_On_Types_Level_Only()
        {
            var one = _fixture.Create<OneSealed>();
            var other = one.DeepClone();
            other.Value = (sbyte)(one.Value + 1);
            one.Another.One = _fixture.Build<OneSealed>().Without(x => x.Another).Create();
            other.Another.One = _fixture.Build<OneSealed>().Without(x => x.Another).Create();

            var expected = one.Another.One.Value.CompareTo(other.Another.One.Value);
            var actual = _comparerOneSealed.Compare(one, other);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Cross_Reference_Should_Not_Fail()
        {
            var other = new SelfSealed();
            var one = new SelfSealed
            {
                First = other,
                Second = other
            };
            other.First = one;
            other.Second = one;

            var expected = SelfSealed.Comparer.Compare(one, other);
            var actual = _comparerSelfSealed.Compare(one, other);

            expected.Should().Be(0);
            actual.Should().Be(expected);
        }

        [Fact]
        public void Detects_Cycle_On_Second_Member_Loop()
        {
            var one = new SelfSealed();
            one.Second = new SelfSealed
            {
                First = one
            };
            /*
                  1
                 / \
                N   2
            cycle: / \
                  1   N
                 / \ 
                N   2
            */

            var other = new SelfSealed
            {
                Second = new SelfSealed
                {
                    First = new SelfSealed()
                }
            };
            /*
                  3
                 / \
                N   4
                   / \
                  5   N
                 / \
                N   N difference here: 2 > N
            */

            var expected = SelfSealed.Comparer.Compare(one, other);
            var actual = _comparerSelfSealed.Compare(one, other);

            expected.Should().Be(1);
            actual.Should().Be(expected);
        }

        [Fact]
        public void Object_With_Bigger_Cycle_Is_Bigger()
        {
            var one = new SelfSealed();
            one.First = new SelfSealed
            {
                First = new SelfSealed
                {
                    First = one
                }
            };
            var other = new SelfSealed { First = one };

            var expected = SelfSealed.Comparer.Compare(one, other);
            var actual = _comparerSelfSealed.Compare(one, other);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Opened_Class_Comparer_Uses_Context_Compare_Method()
        {
            var one = _fixture.Create<SelfOpened>();
            one.Self = one;
            var other = _fixture.Create<SelfOpened>();
            other.Self = other;

            var expected = one.Value.CompareTo(other.Value);
            var actual = _comparerSelfOpened.Compare(one, other);

            actual.Should().Be(expected);
        }

        private readonly Fixture _fixture;
        private readonly IComparer<SelfSealed> _comparerSelfSealed;
        private readonly IComparer<SelfOpened> _comparerSelfOpened;
        private readonly IComparer<OneSealed> _comparerOneSealed;
    }
}
