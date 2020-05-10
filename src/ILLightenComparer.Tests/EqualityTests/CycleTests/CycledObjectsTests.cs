using System;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Force.DeepCloner;
using ILLightenComparer.Tests.ComparerTests.CycleTests.Samples;
using Xunit;

namespace ILLightenComparer.Tests.EqualityTests.CycleTests
{
    public sealed class CycledObjectsTests
    {
        public CycledObjectsTests()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public void Comparison_should_not_fail_because_of_generating_comparers_for_two_dependent_classes()
        {
            var one = _fixture.Create<OneSealed>();
            var other = _fixture.Create<OneSealed>();
            one.Two.Three.One = one;
            other.Two.Three.One = one;

            ComparerForOneSealed.Equals(one, other).Should().BeFalse();
        }

        [Fact]
        public void Hasing_should_fail_because_of_generating_comparers_for_two_dependent_classes()
        {
            var one = _fixture.Create<OneSealed>();
            var other = _fixture.Create<OneSealed>();
            one.Two.Three.One = one;
            other.Two.Three.One = one;

            using (new AssertionScope()) {
                Assert.Throws<ArgumentException>(() => ComparerForOneSealed.GetHashCode(one));
                Assert.Throws<ArgumentException>(() => ComparerForOneSealed.GetHashCode(other));
            }
        }

        [Fact]
        public void Comparison_with_cycle_on_types_level_only()
        {
            var x = _fixture.Create<OneSealed>();
            var y = x.DeepClone();

            y.Value = (sbyte)(x.Value + 1);
            x.Two.Three.One = _fixture.Build<OneSealed>().Without(x => x.Two).Create();
            y.Two.Three.One = _fixture.Build<OneSealed>().Without(x => x.Two).Create();

            var expectedHashX = x.GetHashCode();
            var expectedHashY = y.GetHashCode();
            var expectedEquals = x.Equals(y);

            var hashX = ComparerForOneSealed.GetHashCode(x);
            var hashY = ComparerForOneSealed.GetHashCode(y);
            var equals = ComparerForOneSealed.Equals(x, y);

            using (new AssertionScope()) {
                equals.Should().Be(expectedEquals);
                hashX.Should().Be(expectedHashX);
                hashY.Should().Be(expectedHashY);
            }
        }

        //[Fact]
        //public void Cross_reference_should_not_fail()
        //{
        //    var other = new SelfSealed();
        //    var one = new SelfSealed {
        //        First = other,
        //        Second = other
        //    };
        //    other.First = one;
        //    other.Second = one;

        //    var expected = SelfSealed.Comparer.Equals(one, other);
        //    var actual = ComparerSelfSealed.Equals(one, other);

        //    using (new AssertionScope()) {
        //        expected.Should().Be(0);
        //        actual.Should().Be(expected);
        //    }
        //}

        //[Fact]
        //public void Cycle_detection_in_multiple_threads_works()
        //{
        //    Helper.Parallel(() => {
        //        var comparer = new ComparerBuilder().GetComparer<OneSealed>();

        //        var one = _fixture.Create<OneSealed>();
        //        var other = _fixture.Create<OneSealed>();
        //        one.Two.Three.One = one;
        //        other.Two.Three.One = other;

        //        var expected = one.Value.CompareTo(other.Value);
        //        var actual = comparer.Equals(one, other);

        //        actual.Should().Be(expected);
        //    });
        //}

        //[Fact]
        //public void Detects_cycle_on_second_member()
        //{
        //    var one = new SelfSealed { Value = 1 };
        //    one.Second = new SelfSealed {
        //        First = one,
        //        Value = 2
        //    };
        //    /*
        //          1
        //         / \
        //        N   2
        //    cycle: / \
        //          1   N
        //         / \ 
        //        N   2
        //    */

        //    var other = new SelfSealed {
        //        Second = new SelfSealed {
        //            First = new SelfSealed { Value = 3 },
        //            Value = 4
        //        },
        //        Value = 5
        //    };
        //    /*
        //          3
        //         / \
        //        N   4
        //           / \
        //          5   N
        //         / \
        //        N   N difference here: 2 > N
        //    */

        //    var expected = SelfSealed.Comparer.Equals(one, other);
        //    var actual = ComparerSelfSealed.Equals(one, other);

        //    using (new AssertionScope()) {
        //        expected.Should().Be(1);
        //        actual.Should().Be(expected);
        //    }
        //}

        //[Fact]
        //public void No_cycle_when_identical_objects_in_collection()
        //{
        //    var one = new SelfSealed { Value = 1 };
        //    one.Second = new SelfSealed {
        //        First = one,
        //        Value = 1
        //    };
        //    var two = new SelfSealed { Value = 1 };
        //    two.Second = new SelfSealed {
        //        First = two,
        //        Value = 1
        //    };
        //    var x = new[] {
        //        one,
        //        one,
        //        one
        //    };

        //    var other = new SelfSealed {
        //        Second = new SelfSealed {
        //            First = new SelfSealed {
        //                Value = 4
        //            },
        //            Value = 4
        //        },
        //        Value = 3
        //    };
        //    var y = new[] {
        //        two,
        //        two,
        //        other
        //    };

        //    var expected = SelfSealed.Comparer.Equals(x, y);
        //    var comparer = new ComparerBuilder().For<SelfSealed>(c => c.IgnoreMember(o => o.Id)).GetComparer<SelfSealed[]>();
        //    var actual = comparer.Equals(x, y);

        //    using (new AssertionScope()) {
        //        expected.Should().Be(-1);
        //        actual.Should().Be(-1);
        //    }
        //}

        //[Fact]
        //public void Detects_cycle_handles_equalty_by_value()
        //{
        //    var one = new SelfSealed { Value = 1 };
        //    one.Second = new SelfSealed {
        //        First = one,
        //        Value = 1
        //    };
        //    /*
        //          1-1
        //         / \
        //        N   2-1
        //    cycle: / \
        //          1-1 N
        //         / \ 
        //        N   2-1
        //    */

        //    var other = new SelfSealed {
        //        Second = new SelfSealed {
        //            First = new SelfSealed {
        //                Second = new SelfSealed { Value = 0 },
        //                Value = 4
        //            },
        //            Value = 4
        //        },
        //        Value = 3
        //    };
        //    /*
        //          3-3
        //         / \
        //        N   4-4
        //           / \
        //          5-4 N
        //         / \
        //        N   6-6
        //    */

        //    var expected = SelfSealed.Comparer.Equals(one, other);
        //    var actual = ComparerSelfSealed.Equals(one, other);

        //    using (new AssertionScope()) {
        //        expected.Should().Be(-1);
        //        actual.Should().Be(expected);
        //    }
        //}

        //[Fact]
        //public void Object_with_bigger_cycle_is_bigger()
        //{
        //    var one = new SelfSealed();
        //    one.First = new SelfSealed {
        //        First = new SelfSealed {
        //            First = one
        //        }
        //    };
        //    var other = new SelfSealed { First = one };

        //    var expected = SelfSealed.Comparer.Equals(one, other);
        //    var actual = ComparerSelfSealed.Equals(one, other);

        //    actual.Should().Be(expected);
        //}

        //[Fact]
        //public void Opened_class_comparer_uses_context_compare_method()
        //{
        //    var one = _fixture.Create<SelfOpened>();
        //    one.Self = one;
        //    var other = _fixture.Create<SelfOpened>();
        //    other.Self = other;

        //    var expected = one.Value.CompareTo(other.Value);
        //    var actual = ComparerSelfOpened.Equals(one, other);

        //    actual.Should().Be(expected);
        //}

        //[Fact]
        //public void When_sealed_comparable_has_member_with_cycle()
        //{
        //    var comparer = _builder.For<ComparableChildObject<OneSealed>>().GetComparer();
        //    ComparableChildObject<OneSealed>.ChildComparer = ComparerForOneSealed;

        //    var one = _fixture.Create<OneSealed>();
        //    var other = _fixture.Create<OneSealed>();
        //    one.Value = other.Value;
        //    one.Two.Three.One = one;
        //    other.Two.Three.One = _fixture.Create<OneSealed>();
        //    other.Two.Three.One.Value = one.Value;
        //    other.Two.Three.One.Two.Three.One = other;
        //    var x = new ComparableChildObject<OneSealed> {
        //        ChildField = null,
        //        ChildProperty = other
        //    };
        //    var y = new ComparableChildObject<OneSealed> {
        //        ChildField = null,
        //        ChildProperty = one
        //    };

        //    var expected = ComparerForOneSealed.Equals(other, one);
        //    var actual = comparer.Equals(x, y);

        //    using (new AssertionScope()) {
        //        actual.Should().Be(expected);
        //        actual.Should().BePositive();
        //    }
        //}

        private IEqualityComparer<SelfSealed> ComparerSelfSealed => _builder.For<SelfSealed>(c => c.IgnoreMember(o => o.Id)).GetEqualityComparer();
        private readonly Fixture _fixture;
        private IEqualityComparer<SelfOpened> ComparerSelfOpened => _builder.For<SelfOpened>().GetEqualityComparer();
        private IEqualityComparer<OneSealed> ComparerForOneSealed => _builder.For<OneSealed>().GetEqualityComparer();
        private readonly IComparerBuilder _builder = new ComparerBuilder();
    }
}
