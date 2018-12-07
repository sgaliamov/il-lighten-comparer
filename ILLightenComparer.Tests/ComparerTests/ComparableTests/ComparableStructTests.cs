using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Force.DeepCloner;
using ILLightenComparer.Tests.ComparerTests.ComparableTests.Samples;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests.ComparableTests
{
    public class ComparableStructTests : BaseComparerTests<ContainerStruct>
    {
        [Fact]
        public void Replaced_Comparable_Object_Is_Compared()
        {
            var one = new ContainerStruct
            {
                ComparableProperty = Fixture.Create<ComparableObject>()
            };
            TypedComparer.Compare(one, one.DeepClone()).Should().Be(0);

            for (var i = 0; i < 100; i++)
            {
                one.ComparableProperty = Fixture.Create<ChildComparableObject>();
                var other = new ContainerStruct
                {
                    ComparableProperty = Fixture.Create<ChildComparableObject>()
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
