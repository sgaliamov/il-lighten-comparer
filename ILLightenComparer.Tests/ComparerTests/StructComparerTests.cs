using System.Collections;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using ILLightenComparer.Tests.Samples;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests
{
    public sealed class StructComparerTests : BaseComparerTests<SampleStruct>
    {
        [Fact]
        public void Comparison_Of_Null_With_Object_Produces_Negative_Value()
        {
            var obj = Fixture.Create<SampleStruct>();

            BasicComparer.Compare(default, obj).Should().BeLessThan(0);
        }

        [Fact]
        public void Comparison_Of_Object_With_Null_Produces_Positive_Value()
        {
            var obj = Fixture.Create<SampleStruct>();

            BasicComparer.Compare(obj, default).Should().BeGreaterThan(0);
        }

        private readonly ComparersBuilder _comparersBuilder =
            new ComparersBuilder()
                .SetConfiguration(
                    new CompareConfiguration
                    {
                        IncludeFields = true
                    });

        protected override IComparer BasicComparer => _comparersBuilder.CreateComparer(typeof(SampleStruct));
        protected override IComparer<SampleStruct> TypedComparer => _comparersBuilder.CreateComparer<SampleStruct>();
        protected override IComparer<SampleStruct> ReferenceComparer { get; } = SampleStruct.SampleStructComparer;
    }
}
