using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Force.DeepCloner;
using ILLightenComparer.Tests.ComparerTests.ComparableTests.Samples;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests.ComparableTests
{
    public sealed class ComparableStructTests : BaseComparerTests<ContainerStruct>
    {
        public ComparableStructTests()
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
        public void Replaced_Comparable_Object_Is_Compared_With_Custom_Implementation()
        {
            var one = new ContainerStruct
            {
                ComparableField = Fixture.Create<ComparableObject>()
            };
            TypedComparer.Compare(one, one.DeepClone()).Should().Be(0);

            for (var i = 0; i < 100; i++)
            {
                one.ComparableField = Fixture.Create<ComparableChildObject>();
                var other = new ContainerStruct
                {
                    ComparableField = Fixture.Create<ComparableChildObject>()
                };

                var expected = ContainerStruct.Comparer.Compare(one, other).Normalize();
                var actual = TypedComparer.Compare(one, other).Normalize();

                actual.Should().Be(expected);
            }

            ComparableObject.UsedCompareTo.Should().BeTrue();
        }

        protected override IComparer<ContainerStruct> ReferenceComparer => ContainerStruct.Comparer;
    }
}
