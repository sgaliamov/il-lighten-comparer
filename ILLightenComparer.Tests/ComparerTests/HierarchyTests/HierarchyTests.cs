using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples;
using ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples.Nested;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests
{
    public sealed class HierarchyTests
    {
        public HierarchyTests()
        {
            _comparer = new ComparerBuilder()
                        .Configure(builder => builder.ConfigureFor<NestedStruct>(config => config.MembersOrder(new[]
                        {
                            nameof(NestedStruct.Property),
                            nameof(NestedStruct.NullableProperty)
                        })))
                        .For<HierarchicalObject>()
                        .Configure(config => config.IgnoredMembers(new[]
                        {
                            nameof(HierarchicalObject.ComparableField),
                            nameof(HierarchicalObject.Value),
                            nameof(HierarchicalObject.FirstProperty),
                            nameof(HierarchicalObject.SecondProperty),
                            nameof(HierarchicalObject.NestedField)
                        }))
                        .GetComparer();
        }

        [Fact]
        public void Compare_nested_null_structs()
        {
            var one = new HierarchicalObject();
            var other = new HierarchicalObject
            {
                NestedNullableStructProperty = _fixture.Create<NestedStruct>()
            };

            var expected = HierarchicalObject.Comparer.Compare(one, other);
            expected.Should().Be(-1);
            var actual = _comparer.Compare(one, other);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Compare_nested_structs()
        {
            for (var i = 0; i < 10; i++)
            {
                var one = new HierarchicalObject
                {
                    NestedStructField = _fixture.Create<NestedStruct>(),
                    NestedNullableStructProperty = _fixture.Create<NestedStruct>()
                };

                var other = new HierarchicalObject
                {
                    NestedStructField = _fixture.Create<NestedStruct>(),
                    NestedNullableStructProperty = _fixture.Create<NestedStruct>()
                };

                var expected = HierarchicalObject.Comparer.Compare(one, other);
                var actual = _comparer.Compare(one, other);

                actual.Should().Be(expected);
            }
        }

        [Fact]
        public void Run_generic_tests()
        {
            var builder = new ComparerBuilder()
                          .For<SealedNestedObject>(config => config.MembersOrder(new[]
                          {
                              nameof(SealedNestedObject.DeepNestedField),
                              nameof(SealedNestedObject.DeepNestedProperty),
                              nameof(SealedNestedObject.Key),
                              nameof(SealedNestedObject.Text)
                          }))
                          .For<NestedStruct>(config => config.MembersOrder(new[]
                          {
                              nameof(NestedStruct.Property),
                              nameof(NestedStruct.NullableProperty)
                          }))
                          .For<HierarchicalObject>(config => config.MembersOrder(new[]
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
                          }))
                          .Builder;

            new GenericTests(builder).GenericTest(typeof(HierarchicalObject), HierarchicalObject.Comparer, false, Constants.BigCount);
        }

        private readonly IComparer<HierarchicalObject> _comparer;
        private readonly Fixture _fixture = FixtureBuilder.GetInstance();
    }
}
