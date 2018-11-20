using System.Collections;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using ILLightenComparer.Tests.Samples;
using Xunit;

namespace ILLightenComparer.Tests.Comparers
{
    public sealed class ClassComparerTests : BaseComparerTests<SampleObject>
    {
        [Fact]
        public void Comparison_Of_Null_With_Object_Produces_Negative_Value()
        {
            var obj = Fixture.Create<SampleObject>();

            BasicComparer.Compare(default(SampleObject), obj).Should().BeLessThan(0);
            TypedComparer.Compare(default, obj).Should().BeLessThan(0);
        }

        [Fact]
        public void Comparison_Of_Object_With_Null_Produces_Positive_Value()
        {
            var obj = Fixture.Create<SampleObject>();

            BasicComparer.Compare(obj, default(SampleObject)).Should().BeGreaterThan(0);
            TypedComparer.Compare(obj, default).Should().BeGreaterThan(0);
        }

        private readonly ComparersBuilder _comparersBuilder =
            new ComparersBuilder()
                .SetConfiguration(
                    new CompareConfiguration
                    {
                        IncludeFields = true
                    });

        protected override IComparer BasicComparer => _comparersBuilder.CreateComparer(typeof(SampleObject));
        protected override IComparer<SampleObject> TypedComparer => _comparersBuilder.CreateComparer<SampleObject>();
        protected override IComparer<SampleObject> ReferenceComparer { get; } = SampleObject.SampleObjectComparer;
    }
}
