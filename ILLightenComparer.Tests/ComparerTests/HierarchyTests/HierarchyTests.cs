using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
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
                .For<NestedStruct>()
                .DefineConfiguration(new ComparerSettings
                {
                    MembersOrder = new[]
                    {
                        nameof(NestedStruct.Property),
                        nameof(NestedStruct.NullableProperty)
                    }
                })
                .For<HierarchicalObject>()
                .DefineConfiguration(new ComparerSettings
                {
                    MembersOrder = new[]
                    {
                        nameof(HierarchicalObject.ComparableField),
                        nameof(HierarchicalObject.Value),
                        nameof(HierarchicalObject.FirstProperty),
                        nameof(HierarchicalObject.SecondProperty),
                        nameof(HierarchicalObject.NestedField),
                        nameof(HierarchicalObject.NestedStructField),
                        nameof(HierarchicalObject.NestedNullableStructField),
                        nameof(HierarchicalObject.NestedStructProperty),
                        nameof(HierarchicalObject.NestedNullableStructProperty)
                    }
                });

            _nestedStructComparer = new ComparersBuilder()
                                    .DefineDefaultConfiguration(new ComparerSettings
                                    {
                                        IncludeFields = true
                                    })
                                    .For<NestedStruct>()
                                    .DefineConfiguration(new ComparerSettings
                                    {
                                        MembersOrder = new[]
                                        {
                                            nameof(NestedStruct.Property),
                                            nameof(NestedStruct.NullableProperty)
                                        }
                                    })
                                    .For<HierarchicalObject>()
                                    .DefineConfiguration(new ComparerSettings
                                    {
                                        IgnoredMembers = new[]
                                        {
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
            for (var i = 0; i < 10; i++)
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
        }

        private readonly IComparer<HierarchicalObject> _nestedStructComparer;

        protected override IComparer<HierarchicalObject> ReferenceComparer { get; } = HierarchicalObject.Comparer;
    }
}
