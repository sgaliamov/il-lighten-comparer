using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples.Cycled;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests
{
    public class CycledStructTests
    {
        public CycledStructTests()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var builder = new ComparersBuilder()
                .DefineDefaultConfiguration(new ComparerSettings
                {
                    IncludeFields = true,
                    DetectCycles = true
                });

            _comparerStruct = builder
                              .For<CycledStruct>()
                              .DefineConfiguration(new ComparerSettings
                              {
                                  MembersOrder = new[]
                                  {
                                      nameof(CycledStruct.Value),
                                      nameof(CycledStruct.FirstObject),
                                      nameof(CycledStruct.SecondObject)
                                  }
                              })
                              .GetComparer();

            _comparerObject = builder
                              .For<CycledStructObject>()
                              .DefineConfiguration(new ComparerSettings
                              {
                                  MembersOrder = new[]
                                  {
                                      nameof(CycledStructObject.Text),
                                      nameof(CycledStructObject.FirstStruct),
                                      nameof(CycledStructObject.SecondStruct)
                                  }
                              })
                              .GetComparer();
        }

        [Fact]
        public void Cycle_In_Object()
        {
            var one = new CycledStructObject
            {
                FirstStruct = new CycledStruct()
            };
            one.FirstStruct.SecondObject = one;

            var other = new CycledStructObject
            {
                FirstStruct = new CycledStruct()
            };
            other.FirstStruct.SecondObject = other;

            var expected = CycledStructObject.Comparer.Compare(one, other);
            var actual = _comparerObject.Compare(one, other);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Cycle_In_Struct()
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

            actual.Should().Be(expected);
        }

        private readonly Fixture _fixture;
        private readonly IComparer<CycledStruct> _comparerStruct;
        private readonly IComparer<CycledStructObject> _comparerObject;
    }
}
