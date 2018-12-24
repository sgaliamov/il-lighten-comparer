using System;
using System.Reflection;
using AutoFixture;
using FluentAssertions;
using Force.DeepCloner;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests
{
    public sealed class SampleComparableTests
    {
        [Fact]
        public void Compare_Sample_Comparable_Objects()
        {
            Test(typeof(SampleComparableBaseObject<>), nameof(SampleComparableBaseObject<object>.Comparer));
            Test(typeof(SampleComparableChildObject<>), nameof(SampleComparableChildObject<object>.ChildComparer));

            foreach (var item in SampleTypes.Types)
            {
                typeof(SampleComparableBaseObject<>)
                    .MakeGenericType(item.Key)
                    .GetField(nameof(SampleComparableBaseObject<object>.UsedCompareTo), BindingFlags.Public | BindingFlags.Static)
                    .GetValue(null)
                    .Should()
                    .Be(true);
            }
        }

        [Fact]
        public void Compare_Sample_Comparable_Structs()
        {
            Test(typeof(SampleComparableStruct<>), nameof(SampleComparableStruct<object>.Comparer));
        }

        [Fact]
        public void Custom_Comparable_Implementation_Should_Return_Negative_When_First_Argument_IsNull()
        {
            var one = new SampleObject<SampleComparableBaseObject<EnumSmall>>
            {
                Property = FixtureBuilder.GetInstance().Create<SampleComparableBaseObject<EnumSmall>>()
            };

            var other = one.DeepClone();
            one.Property = null;

            var comparer = new ComparersBuilder().GetComparer<SampleObject<SampleComparableBaseObject<EnumSmall>>>();

            comparer.Compare(one, other).Should().BeNegative();
        }

        [Fact]
        public void Replaced_Comparable_Object_Is_Compared_With_Custom_Implementation()
        {
            var comparer = new ComparersBuilder().GetComparer<SampleObject<SampleComparableBaseObject<EnumSmall>>>();
            var fixture = FixtureBuilder.GetInstance();

            var one = new SampleObject<SampleComparableBaseObject<EnumSmall>>
            {
                Property = fixture.Create<SampleComparableBaseObject<EnumSmall>>()
            };
            comparer.Compare(one, one.DeepClone()).Should().Be(0);

            for (var i = 0; i < Constants.SmallCount; i++)
            {
                one.Property = fixture.Create<SampleComparableBaseObject<EnumSmall>>();
                var other = new SampleObject<SampleComparableBaseObject<EnumSmall>>
                {
                    Property = fixture.Create<SampleComparableBaseObject<EnumSmall>>()
                };

                var expected = one.Property.CompareTo(other.Property).Normalize();
                var actual = comparer.Compare(one, other).Normalize();

                actual.Should().Be(expected);
            }

            SampleComparableChildObject<EnumSmall>.UsedCompareTo.Should().BeTrue();
        }

        private static void Test(Type objectGenericType, string comparerName)
        {
            foreach (var item in SampleTypes.Types)
            {
                var objectType = objectGenericType.MakeGenericType(item.Key);

                if (item.Value != null)
                {
                    objectType
                        .GetField(comparerName, BindingFlags.Public | BindingFlags.Static)
                        .SetValue(null, item.Value);
                }

                GenericTests.GenericTest(objectType, null, false, Constants.SmallCount);
            }
        }
    }
}
