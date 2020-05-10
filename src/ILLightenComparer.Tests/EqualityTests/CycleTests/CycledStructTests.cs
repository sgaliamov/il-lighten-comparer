using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Execution;
using ILLightenComparer.Tests.EqualityTests.CycleTests.Samples;
using Xunit;

namespace ILLightenComparer.Tests.EqualityTests.CycleTests
{
    public sealed class CycledStructTests
    {
        public CycledStructTests()
        {
            _builder = new ComparerBuilder(config => config
                .DefineMembersOrder<CycledStruct>(order => order
                    .Member(o => o.Property)
                    .Member(o => o.FirstObject)
                    .Member(o => o.SecondObject)))
                .For<CycledStructObject>(config => config
                .DefineMembersOrder(order => order
                    .Member(o => o.TextField)
                    .Member(o => o.FirstStruct)
                    .Member(o => o.SecondStruct)))
                .Builder;
        }

        [Fact]
        public void Detects_cycle_in_object()
        {
            var one = new CycledStructObject();
            one.FirstStruct = new CycledStruct { SecondObject = one };

            var other = new CycledStructObject();
            other.FirstStruct = new CycledStruct { SecondObject = other };

            using (new AssertionScope()) {
                Assert.Throws<ArgumentException>(() => ComparerObject.Equals(one, other));
                Assert.Throws<ArgumentException>(() => ComparerObject.GetHashCode(one));
                Assert.Throws<ArgumentException>(() => ComparerObject.GetHashCode(other));
            }
        }

        [Fact]
        public void Detects_cycle_in_struct()
        {
            var one = new CycledStruct {
                FirstObject = new CycledStructObject()
            };
            one.FirstObject.SecondStruct = one;

            var other = new CycledStruct {
                FirstObject = new CycledStructObject()
            };
            other.FirstObject.SecondStruct = other;

            using (new AssertionScope()) {
                Assert.Throws<ArgumentException>(() => ComparerStruct.Equals(one, other));
                Assert.Throws<ArgumentException>(() => ComparerStruct.GetHashCode(one));
                Assert.Throws<ArgumentException>(() => ComparerStruct.GetHashCode(other));
            }
        }

        [Fact]
        public void Detects_cycle_on_first_member()
        {
            var one = new CycledStructObject();
            /*
                      1
                     / \
                    vX  vX
                   / |  | \ 
                  N  N  N  N
            */

            var other = new CycledStructObject();
            other.FirstStruct = new CycledStruct {
                FirstObject = other
            };
            /*
                      2
                     / \
                    v1  vX
                   / |  | \ 
                  2  N  N  N
            */

            using (new AssertionScope()) {
                ComparerObject.Equals(one, other).Should().BeFalse();
                ComparerObject.GetHashCode(one).Should().Be(0);
                Assert.Throws<ArgumentException>(() => ComparerObject.GetHashCode(other));
            }
        }

        [Fact]
        public void Detects_cycle_on_second_member()
        {
            var one = new CycledStruct { SecondObject = new CycledStructObject() };
            one.SecondObject.FirstStruct = one;
            /*
                  v1
                 /  \
                N    1
                   /  \
                  v1   vX
                 / |   | \
                N  1   N  N
            */

            var other = new CycledStruct {
                SecondObject = new CycledStructObject()
            };
            /*
                  v2
                 /  \
                N    2
                   /  \
                  v2   VY
                 / |   | \
                N  N   N  N
            */

            using (new AssertionScope()) {
                ComparerStruct.Equals(one, other).Should().BeFalse();
                Assert.Throws<ArgumentException>(() => ComparerStruct.GetHashCode(one));
                ComparerStruct.GetHashCode(other).Should().Be(193376997);
            }
        }

        private readonly IComparerBuilder _builder;
        private IEqualityComparer<CycledStruct> ComparerStruct => _builder.GetEqualityComparer<CycledStruct>();
        private IEqualityComparer<CycledStructObject> ComparerObject => _builder.GetEqualityComparer<CycledStructObject>();
    }
}
