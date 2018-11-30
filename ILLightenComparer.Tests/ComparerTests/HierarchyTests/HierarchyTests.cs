using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Force.DeepCloner;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests
{
    public sealed class HierarchyTests : BaseComparerTests<ContainerObject>
    {
        public HierarchyTests()
        {
            ComparersBuilder
                .DefineConfiguration(typeof(NestedObject),
                    new ComparerSettings
                    {
                        MembersOrder = new[]
                        {
                            nameof(NestedObject.DeepNestedField),
                            nameof(NestedObject.Key),
                            nameof(NestedObject.DeepNestedProperty)
                        }
                    })
                .For<ContainerObject>()
                .DefineConfiguration(new ComparerSettings
                {
                    MembersOrder = new[]
                    {
                        nameof(ContainerObject.ComparableProperty),
                        nameof(ContainerObject.ComparableField),
                        nameof(ContainerObject.Value),
                        nameof(ContainerObject.FirstProperty),
                        nameof(ContainerObject.SecondProperty),
                        nameof(ContainerObject.NestedField)
                    }
                });
        }

        [Fact]
        public void Custom_Comparable_Implementation_Should_Be_Used()
        {
            TypedComparer.Compare(
                             Fixture.Create<ContainerObject>(),
                             Fixture.Create<ContainerObject>())
                         .Should()
                         .NotBe(0);

            ComparableNestedObject.UsedCompareTo.Should().BeTrue();
        }

        [Fact]
        public void Custom_Comparable_Implementation_Should_Return_Negative_When_First_Argument_IsNull()
        {
            var other = Fixture.Create<ContainerObject>();
            var one = other.DeepClone();
            one.ComparableProperty = null;

            TypedComparer.Compare(one, other)
                         .Should()
                         .BeNegative();
        }

        protected override IComparer<ContainerObject> ReferenceComparer { get; } = ContainerObject.Comparer;
    }
}
