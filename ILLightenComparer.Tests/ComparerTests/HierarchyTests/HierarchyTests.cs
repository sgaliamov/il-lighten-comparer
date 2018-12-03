using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Force.DeepCloner;
using ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples;
using ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples.Nested;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests
{
    public sealed class HierarchyTests : BaseComparerTests<HierarchicalObject>
    {
        public HierarchyTests()
        {
            ComparersBuilder
                .DefineConfiguration(typeof(SealedNestedObject),
                    new ComparerSettings
                    {
                        MembersOrder = new[]
                        {
                            nameof(SealedNestedObject.DeepNestedField),
                            nameof(SealedNestedObject.DeepNestedProperty),
                            nameof(SealedNestedObject.Key),
                            nameof(SealedNestedObject.Text)
                        }
                    })
                .For<HierarchicalObject>()
                .DefineConfiguration(new ComparerSettings
                {
                    MembersOrder = new[]
                    {
                        nameof(HierarchicalObject.ComparableProperty),
                        nameof(HierarchicalObject.ComparableField),
                        nameof(HierarchicalObject.Value),
                        nameof(HierarchicalObject.FirstProperty),
                        nameof(HierarchicalObject.SecondProperty),
                        nameof(HierarchicalObject.NestedField)
                    }
                });
        }

        [Fact]
        public void Custom_Comparable_Implementation_Should_Be_Used()
        {
            var nestedObject = Fixture.Create<ComparableObject>();
            var other = Fixture.Create<HierarchicalObject>();
            other.ComparableProperty = nestedObject;
            var one = other.DeepClone();
            one.ComparableProperty.Value = other.ComparableProperty.Value + 1;

            TypedComparer.Compare(one, other)
                         .Should()
                         .NotBe(0);

            ComparableObject.UsedCompareTo.Should().BeTrue();
        }

        [Fact]
        public void Custom_Comparable_Implementation_Should_Return_Negative_When_First_Argument_IsNull()
        {
            var nestedObject = Fixture.Create<ComparableObject>();
            var other = Fixture.Create<HierarchicalObject>();
            other.ComparableProperty = nestedObject;
            var one = other.DeepClone();
            one.ComparableProperty = null;

            TypedComparer.Compare(one, other)
                         .Should()
                         .BeNegative();
        }

        protected override IComparer<HierarchicalObject> ReferenceComparer { get; } = HierarchicalObject.Comparer;
    }
}
