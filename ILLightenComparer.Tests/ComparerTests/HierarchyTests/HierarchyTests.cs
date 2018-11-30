using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
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
                        nameof(ContainerObject.Comparable),
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
            var one = Fixture.Build<ContainerObject>()
                             .Without(x => x.Comparable)
                             .Create();

            var other = Fixture.Build<ContainerObject>()
                               .With(x => x.Comparable, new ComparableNestedObject())
                               .Create();

            TypedComparer.Compare(one, other)
                         .Should()
                         .BeNegative();
        }

        protected override IComparer<ContainerObject> ReferenceComparer { get; } = ContainerObject.Comparer;
    }
}
