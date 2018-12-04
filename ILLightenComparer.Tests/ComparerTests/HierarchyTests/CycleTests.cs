using System.Collections.Generic;
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
        [Fact(Skip = "Not implemented yet")]
        public void Cycle_In_Struct()
        {
            var nestedObject = new ObjectWithCycledStruct
            {
                Value = new CycledStruct
                {
                    Object = new ObjectWithCycledStruct()
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

        [Fact]
        public void Detects_Cycle_On_Second_Loop()
        {
            var one = new SelfSealed();
            one.Second = new SelfSealed
            {
                First = one
            };
            var other = _fixture.Create<SelfSealed>();
            other.Second = new SelfSealed
            {
                First = new SelfSealed()
            };

            var expected = SelfSealed.Comparer.Compare(one, other);
            var actual = ComparerSelfSealed.Compare(one, other);

            actual.Should().Be(expected);
        }

        [Fact(Skip = "Not implemented yet")]
        public void Nested_Sealed_Comparison_Should_Not_Fail()
        {
            var one = _fixture.Create<OneSealed>();

            var other = _fixture.Create<OneSealed>();

            //var expected = OneSealed.Comparer.Compare(one, other);
            //var actual = ComparerOneSealed.Compare(one, other);

            //actual.Should().Be(expected);
        }

        [Fact(Skip = "Not implemented yet")]
        public void Not_Cycle_When_Second_Property_Is_Same()
        {
            var other = new SelfSealed
            {
                First = new SelfSealed(),
                Second = new SelfSealed()
            };
            var one = new SelfSealed
            {
                First = other,
                Second = other
            };

            var expected = SelfSealed.Comparer.Compare(one, other);
            var actual = ComparerSelfSealed.Compare(one, other);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Self_Sealed_Comparison_Should_Not_Fail()
        {
            var one = new SelfSealed();
            one.First = new SelfSealed
            {
                First = new SelfSealed
                {
                    First = one
                }
            };
            var other = _fixture.Create<SelfSealed>();
            other.First = one;

            var expected = SelfSealed.Comparer.Compare(one, other);
            var actual = ComparerSelfSealed.Compare(one, other);

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

        private IComparer<SelfSealed> ComparerSelfSealed =>
            _builder.For<SelfSealed>()
                    .DefineConfiguration(new ComparerSettings())
                    .GetComparer();

        private IComparer<OneSealed> ComparerOneSealed =>
            _builder.For<OneSealed>()
                    .DefineConfiguration(new ComparerSettings())
                    .GetComparer();
    }
}
