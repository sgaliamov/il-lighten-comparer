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
                        nameof(HierarchicalObject.NestedField),
                        nameof(HierarchicalObject.NestedStructField),
                        nameof(HierarchicalObject.NestedNullableStructProperty)
                    }
                });

            _nestedStructComparer = new ComparersBuilder()
                                    .DefineDefaultConfiguration(new ComparerSettings
                                    {
                                        IncludeFields = true
                                    })
                                    .For<HierarchicalObject>()
                                    .DefineConfiguration(new ComparerSettings
                                    {
                                        IgnoredMembers = new[]
                                        {
                                            nameof(HierarchicalObject.ComparableProperty),
                                            nameof(HierarchicalObject.ComparableField),
                                            nameof(HierarchicalObject.Value),
                                            nameof(HierarchicalObject.FirstProperty),
                                            nameof(HierarchicalObject.SecondProperty),
                                            nameof(HierarchicalObject.NestedField)
                                        }
                                    })
                                    .GetComparer();
        }

        [Fact]
        public void Compare_Nested_Null_Structs()
        {
            var one = new HierarchicalObject();

            var other = new HierarchicalObject
            {
                NestedNullableStructProperty = Fixture.Create<NestedStruct>()
            };

            var expected = HierarchicalObject.Comparer.Compare(one, other);
            expected.Should().Be(-1);
            var actual = _nestedStructComparer.Compare(one, other);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Compare_Nested_Structs()
        {
            var one = new HierarchicalObject
            {
                NestedStructField = Fixture.Create<NestedStruct>(),
                NestedNullableStructProperty = Fixture.Create<NestedStruct>()
            };

            var other = new HierarchicalObject
            {
                NestedStructField = Fixture.Create<NestedStruct>(),
                NestedNullableStructProperty = Fixture.Create<NestedStruct>()
            };

            var expected = HierarchicalObject.Comparer.Compare(one, other);
            var actual = _nestedStructComparer.Compare(one, other);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Custom_Comparable_Implementation_Should_Be_Used()
        {
            var other = Fixture.Create<HierarchicalObject>();

            var one = other.DeepClone();
            one.ComparableProperty.Value = other.ComparableProperty.Value + 1;

            var expected = HierarchicalObject.Comparer.Compare(one, other);
            var actual = TypedComparer.Compare(one, other);

            expected.Should().Be(1);
            actual.Should().Be(expected);

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

        private readonly IComparer<HierarchicalObject> _nestedStructComparer;

        protected override IComparer<HierarchicalObject> ReferenceComparer { get; } = HierarchicalObject.Comparer;
    }
}
