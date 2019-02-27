using System;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Force.DeepCloner;
using ILLightenComparer.Tests.ComparerTests.CycleTests.Samples;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests.CycleTests
{
    public sealed class CycledObjectsTests
    {
        public CycledObjectsTests()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public void Comparison_Should_Not_Fail_Because_Of_Generating_Comparers_For_Two_Dependent_Classes()
        {
            var one = _fixture.Create<OneSealed>();
            var other = _fixture.Create<OneSealed>();
            one.Two.Three.One = one;
            other.Two.Three.One = one;

            var expected = one.Value.CompareTo(other.Value);
            var actual = ComparerForOneSealed.Compare(one, other);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Comparison_With_Cycle_On_Types_Level_Only()
        {
            var one = _fixture.Create<OneSealed>();
            var other = one.DeepClone();
            other.Value = (sbyte)(one.Value + 1);
            one.Two.Three.One = _fixture.Build<OneSealed>().Without(x => x.Two).Create();
            other.Two.Three.One = _fixture.Build<OneSealed>().Without(x => x.Two).Create();

            var expected = one.Two.Three.One.Value.CompareTo(other.Two.Three.One.Value);
            var actual = ComparerForOneSealed.Compare(one, other);

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
            var actual = ComparerSelfSealed.Compare(one, other);

            expected.Should().Be(0);
            actual.Should().Be(expected);
        }

        [Fact]
        public void Cycle_Detection_In_Multiple_Threads_Works()
        {
            Helper.Parallel(
                () =>
                {
                    var comparer = new ComparerBuilder().GetComparer<OneSealed>();

                    var one = _fixture.Create<OneSealed>();
                    var other = _fixture.Create<OneSealed>();
                    one.Two.Three.One = one;
                    other.Two.Three.One = other;

                    var expected = one.Value.CompareTo(other.Value);
                    var actual = comparer.Compare(one, other);

                    actual.Should().Be(expected);
                },
                Environment.ProcessorCount * 10);
        }

        [Fact]
        public void Detects_Cycle_On_Second_Member()
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
            var actual = ComparerSelfSealed.Compare(one, other);

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
            var actual = ComparerSelfSealed.Compare(one, other);

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
            var actual = ComparerSelfOpened.Compare(one, other);

            actual.Should().Be(expected);
        }

        [Fact]
        public void When_Sealed_Comparable_Has_Member_With_Cycle()
        {
            var comparer = _builder.For<SampleComparableChildObject<OneSealed>>().GetComparer();
            SampleComparableChildObject<OneSealed>.ChildComparer = ComparerForOneSealed;

            var one = _fixture.Create<OneSealed>();
            var other = _fixture.Create<OneSealed>();
            one.Value = other.Value;
            one.Two.Three.One = one;
            other.Two.Three.One = _fixture.Create<OneSealed>();
            other.Two.Three.One.Value = one.Value;
            other.Two.Three.One.Two.Three.One = other;
            var x = new SampleComparableChildObject<OneSealed>
            {
                ChildField = null,
                ChildProperty = other
            };
            var y = new SampleComparableChildObject<OneSealed>
            {
                ChildField = null,
                ChildProperty = one
            };

            var expected = ComparerForOneSealed.Compare(other, one);
            var actual = comparer.Compare(x, y);

            actual.Should().Be(expected);
            actual.Should().BePositive();
        }

        private IComparer<SelfSealed> ComparerSelfSealed =>
            _builder.For<SelfSealed>(c => c.SetIgnoredMembers(new[] { nameof(SelfSealed.Id) }))
                    .GetComparer();

        private readonly Fixture _fixture;
        private IComparer<SelfOpened> ComparerSelfOpened => _builder.For<SelfOpened>().GetComparer();
        private IComparer<OneSealed> ComparerForOneSealed => _builder.For<OneSealed>().GetComparer();
        private readonly IComparerBuilder _builder = new ComparerBuilder();
    }
}
