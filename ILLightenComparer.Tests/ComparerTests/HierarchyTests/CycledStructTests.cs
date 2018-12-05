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

            _comparerStruct = builder.For<CycledStruct>().GetComparer();
            _comparerObject = builder.For<CycledStructObject>().GetComparer();
        }

        [Fact]
        public void Cycle_In_Struct()
        {
            var one = new CycledStruct();
            var other = new CycledStruct();

            var expected = CycledStruct.Comparer.Compare(one, other);
            var actual = _comparerStruct.Compare(one, other);

            actual.Should().Be(expected);
        }

        private readonly Fixture _fixture;
        private readonly IComparer<CycledStruct> _comparerStruct;
        private readonly IComparer<CycledStructObject> _comparerObject;
    }
}
