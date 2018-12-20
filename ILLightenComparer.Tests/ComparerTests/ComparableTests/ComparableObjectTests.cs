using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Force.DeepCloner;
using ILLightenComparer.Tests.ComparerTests.ComparableTests.Samples;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests.ComparableTests
{
    public sealed class ComparableObjectTests : BaseComparerTests<ContainerObject>
    {
        [Fact(Timeout = Constants.DefaultTimeout)]
        public void Custom_Comparable_Implementation_Should_Be_Used()
        {
            var other = new ContainerObject
            {
                ComparableProperty = Fixture.Create<SampleComparableBaseObject<EnumSmall>>()
            };

            var one = other.DeepClone();
            one.ComparableProperty.Property = other.ComparableProperty.Property + 1;

            var expected = ContainerObject.Comparer.Compare(one, other);
            var actual = TypedComparer.Compare(one, other);

            actual.Should().Be(expected);

            SampleComparableBaseObject<EnumSmall>.UsedCompareTo.Should().BeTrue();
        }

        [Fact(Timeout = Constants.DefaultTimeout)]
        public void Custom_Comparable_Implementation_Should_Return_Negative_When_First_Argument_IsNull()
        {
            var other = new ContainerObject
            {
                ComparableProperty = Fixture.Create<SampleComparableBaseObject<EnumSmall>>()
            };

            var one = other.DeepClone();
            one.ComparableProperty = null;

            TypedComparer.Compare(one, other)
                         .Should()
                         .BeNegative();
        }

        [Fact(Timeout = Constants.DefaultTimeout)]
        public void Replaced_Comparable_Object_Is_Compared_With_Custom_Implementation()
        {
            var one = new ContainerObject
            {
                ComparableProperty = Fixture.Create<SampleComparableBaseObject<EnumSmall>>()
            };
            TypedComparer.Compare(one, one.DeepClone()).Should().Be(0);

            for (var i = 0; i < 100; i++)
            {
                one.ComparableProperty = Fixture.Create<SampleComparableChildObject<EnumSmall>>();
                var other = new ContainerObject
                {
                    ComparableProperty = Fixture.Create<SampleComparableChildObject<EnumSmall>>()
                };

                var expected = ContainerObject.Comparer.Compare(one, other).Normalize();
                var actual = TypedComparer.Compare(one, other).Normalize();

                actual.Should().Be(expected);
            }

            SampleComparableChildObject<EnumSmall>.UsedCompareTo.Should().BeTrue();
        }

        protected override IComparer<ContainerObject> ReferenceComparer => ContainerObject.Comparer;
    }
}
