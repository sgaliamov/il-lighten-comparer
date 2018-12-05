using System.Collections.Generic;
using AutoFixture;
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
            var nestedObject = new CycledStructObject
            {
                Value = new CycledStruct
                {
                    Object = new CycledStructObject()
                }
            };
            var cycledStruct = new CycledStruct
            {
                Object = nestedObject
            };
            nestedObject.Value.Object.Value = cycledStruct;

            //var expected = SelfSealed.Comparer.Compare(one, other);
            //var actual = ComparerSelfSealed.Compare(one, other);

            //actual.Should().Be(expected);
        }

        private readonly Fixture _fixture;
        private readonly IComparer<CycledStruct> _comparerStruct;
        private readonly IComparer<CycledStructObject> _comparerObject;
    }
}
