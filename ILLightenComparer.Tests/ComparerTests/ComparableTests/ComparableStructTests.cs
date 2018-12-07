using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Force.DeepCloner;
using ILLightenComparer.Tests.ComparerTests.ComparableTests.Samples;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests.ComparableTests
{
    public class ComparableStructTests : BaseComparerTests<ContainerStruct>
    {
        [Fact]
        public void Replaced_Comparable_Object_Is_Compared()
        {
            var other = Fixture.Create<ContainerStruct>();

            var one = other.DeepClone();
            one.ComparableField = Fixture.Create<ChildComparableObject>();

            var expected = ContainerStruct.Comparer.Compare(one, other);
            var actual = TypedComparer.Compare(one, other);

            actual.Should().Be(expected);
        }

        protected override IComparer<ContainerStruct> ReferenceComparer => ContainerStruct.Comparer;
    }
}
