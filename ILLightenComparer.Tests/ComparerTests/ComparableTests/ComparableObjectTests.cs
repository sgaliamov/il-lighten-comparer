using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Force.DeepCloner;
using ILLightenComparer.Tests.ComparerTests.ComparableTests.Samples;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests.ComparableTests
{
    public class ComparableObjectTests : BaseComparerTests<ContainerObject>
    {
        [Fact]
        public void Custom_Comparable_Implementation_Should_Be_Used()
        {
            var other = Fixture.Create<ContainerObject>();

            var one = other.DeepClone();
            one.ComparableProperty.Property = other.ComparableProperty.Property + 1;

            var expected = ContainerObject.Comparer.Compare(one, other);
            var actual = TypedComparer.Compare(one, other);

            expected.Should().Be(1);
            actual.Should().Be(expected);

            ComparableObject.UsedCompareTo.Should().BeTrue();
        }

        [Fact]
        public void Custom_Comparable_Implementation_Should_Return_Negative_When_First_Argument_IsNull()
        {
            var other = Fixture.Create<ContainerObject>();
            other.ComparableProperty = Fixture.Create<ComparableObject>();

            var one = other.DeepClone();
            one.ComparableProperty = null;

            TypedComparer.Compare(one, other)
                         .Should()
                         .BeNegative();
        }

        [Fact]
        public void Replaced_Comparable_Object_Is_Compared()
        {
            var other = Fixture.Create<ContainerObject>();

            var one = other.DeepClone();
            one.ComparableProperty = Fixture.Create<ChildComparableObject>();

            var expected = ContainerObject.Comparer.Compare(one, other);
            var actual = TypedComparer.Compare(one, other);

            actual.Should().Be(expected);
        }

        protected override IComparer<ContainerObject> ReferenceComparer => ContainerObject.Comparer;
    }
}
