using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Force.DeepCloner;
using ILLightenComparer.Tests.ComparerTests.ComparableTests.Samples;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests.ComparableTests
{
    public class ComparableObjectTests : BaseComparerTests<ContainerObject>
    {
        public ComparableObjectTests()
        {
            ComparersBuilder.For<ComparableChildObject>()
                            .DefineConfiguration(new ComparerSettings
                            {
                                // todo: remove this configuration when simplified comparer will be implemented
                                MembersOrder = new[]
                                {
                                    nameof(ComparableChildObject.Property),
                                    nameof(ComparableChildObject.Field)
                                }
                            });
        }

        [Fact(Timeout = Constants.DefaultTimeout)]
        public void Custom_Comparable_Implementation_Should_Be_Used()
        {
            var other = new ContainerObject
            {
                ComparableProperty = Fixture.Create<ComparableObject>()
            };

            var one = other.DeepClone();
            one.ComparableProperty.Property = other.ComparableProperty.Property + 1;

            var expected = ContainerObject.Comparer.Compare(one, other);
            var actual = TypedComparer.Compare(one, other);

            actual.Should().Be(expected);

            ComparableObject.UsedCompareTo.Should().BeTrue();
        }

        [Fact(Timeout = Constants.DefaultTimeout)]
        public void Custom_Comparable_Implementation_Should_Return_Negative_When_First_Argument_IsNull()
        {
            var other = new ContainerObject
            {
                ComparableProperty = Fixture.Create<ComparableObject>()
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
                ComparableProperty = Fixture.Create<ComparableObject>()
            };
            TypedComparer.Compare(one, one.DeepClone()).Should().Be(0);

            for (var i = 0; i < 100; i++)
            {
                one.ComparableProperty = Fixture.Create<ComparableChildObject>();
                var other = new ContainerObject
                {
                    ComparableProperty = Fixture.Create<ComparableChildObject>()
                };

                var expected = ContainerObject.Comparer.Compare(one, other).Normalize();
                var actual = TypedComparer.Compare(one, other).Normalize();

                actual.Should().Be(expected);
            }

            ComparableObject.UsedCompareTo.Should().BeTrue();
        }

        protected override IComparer<ContainerObject> ReferenceComparer => ContainerObject.Comparer;
    }
}
