using System.Collections.Generic;
using FluentAssertions;
using ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples.Cycled;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests
{
    public class CycledStructTests
    {
        public CycledStructTests()
        {
            var builder = new ComparersBuilder()
                          .DefineDefaultConfiguration(new ComparerSettings
                          {
                              IncludeFields = true,
                              DetectCycles = true
                          })
                          .DefineConfiguration(typeof(CycledStruct),
                              new ComparerSettings
                              {
                                  MembersOrder = new[]
                                  {
                                      nameof(CycledStruct.Value),
                                      nameof(CycledStruct.FirstObject),
                                      nameof(CycledStruct.SecondObject)
                                  }
                              })
                          .DefineConfiguration(typeof(CycledStructObject),
                              new ComparerSettings
                              {
                                  MembersOrder = new[]
                                  {
                                      nameof(CycledStructObject.Text),
                                      nameof(CycledStructObject.FirstStruct),
                                      nameof(CycledStructObject.SecondStruct)
                                  },
                                  IgnoredMembers = new[] { nameof(CycledStructObject.Id) }
                              });

            _comparerStruct = builder.GetComparer<CycledStruct>();
            _comparerObject = builder.GetComparer<CycledStructObject>();
        }

        [Fact]
        public void Detects_Cycle_In_Object()
        {
            var one = new CycledStructObject();
            one.FirstStruct = new CycledStruct { SecondObject = one };

            var other = new CycledStructObject();
            other.FirstStruct = new CycledStruct { SecondObject = other };

            var expected = CycledStructObject.Comparer.Compare(one, other);
            var actual = _comparerObject.Compare(one, other);

            expected.Should().Be(0);
            actual.Should().Be(expected);
        }

        [Fact]
        public void Detects_Cycle_In_Struct()
        {
            var one = new CycledStruct
            {
                FirstObject = new CycledStructObject()
            };
            one.FirstObject.SecondStruct = one;

            var other = new CycledStruct
            {
                FirstObject = new CycledStructObject()
            };
            other.FirstObject.SecondStruct = other;

            var expected = CycledStruct.Comparer.Compare(one, other);
            var actual = _comparerStruct.Compare(one, other);

            expected.Should().Be(0);
            actual.Should().Be(expected);
        }

        [Fact]
        public void Detects_Cycle_On_First_Member()
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
            other.FirstStruct = new CycledStruct
            {
                FirstObject = other
            };
            /*
                      2
                     / \
                    v1  vX
                   / |  | \ 
                  2  N  N  N
            */

            var expected = CycledStructObject.Comparer.Compare(one, other);
            var actual = _comparerObject.Compare(one, other);

            expected.Should().Be(-1);
            actual.Should().Be(expected);
        }

        [Fact]
        public void Detects_Cycle_On_Second_Member()
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

            var other = new CycledStruct
            {
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

            var expected = CycledStruct.Comparer.Compare(one, other);
            var actual = _comparerStruct.Compare(one, other);

            expected.Should().Be(1);
            actual.Should().Be(expected);
        }

        private readonly IComparer<CycledStruct> _comparerStruct;
        private readonly IComparer<CycledStructObject> _comparerObject;
    }
}
