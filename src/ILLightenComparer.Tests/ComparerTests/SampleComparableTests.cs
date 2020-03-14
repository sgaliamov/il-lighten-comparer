using System;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Force.DeepCloner;
using ILLightenComparer.Tests.ComparerTests.Samples;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests
{
    public sealed class SampleComparableTests
    {
        [Fact]
        public void Compare_comparable_objects()
        {
            Test(typeof(SampleComparableBaseObject<>), nameof(SampleComparableBaseObject<object>.Comparer), false);
            Test(typeof(SampleComparableChildObject<>), nameof(SampleComparableChildObject<object>.ChildComparer), false);
            Test(typeof(SampleComparableBaseObject<>), nameof(SampleComparableBaseObject<object>.Comparer), true);
            Test(typeof(SampleComparableChildObject<>), nameof(SampleComparableChildObject<object>.ChildComparer), true);

            foreach (var item in SampleTypes.Types) {
                typeof(SampleComparableBaseObject<>)
                    .MakeGenericType(item.Key)
                    .GetField(nameof(SampleComparableBaseObject<object>.UsedCompareTo), BindingFlags.Public | BindingFlags.Static)
                    .GetValue(null)
                    .Should()
                    .Be(true);
            }
        }

        [Fact]
        public void Compare_comparable_structs()
        {
            Test(typeof(SampleComparableStruct<>), nameof(SampleComparableStruct<object>.Comparer), false);
            Test(typeof(SampleComparableStruct<>), nameof(SampleComparableStruct<object>.Comparer), true);
        }

        [Fact]
        public void Custom_comparable_implementation_should_return_negative_when_first_argument_isnull()
        {
            var one = new SampleObject<SampleComparableBaseObject<EnumSmall>> {
                Property = FixtureBuilder.GetInstance().Create<SampleComparableBaseObject<EnumSmall>>()
            };

            var other = one.DeepClone();
            one.Property = null;

            var comparer = new ComparerBuilder().GetComparer<SampleObject<SampleComparableBaseObject<EnumSmall>>>();

            comparer.Compare(one, other).Should().BeNegative();
        }

        [Fact]
        public void Replaced_comparable_object_is_compared_with_custom_implementation()
        {
            var comparer = new ComparerBuilder().GetComparer<SampleObject<SampleComparableBaseObject<EnumSmall>>>();
            var fixture = FixtureBuilder.GetInstance();

            var one = new SampleObject<SampleComparableBaseObject<EnumSmall>> {
                Property = fixture.Create<SampleComparableChildObject<EnumSmall>>()
            };
            comparer.Compare(one, one.DeepClone()).Should().Be(0);

            for (var i = 0; i < Constants.SmallCount; i++) {
                one.Property = fixture.Create<SampleComparableChildObject<EnumSmall>>();
                var other = new SampleObject<SampleComparableBaseObject<EnumSmall>> {
                    Property = fixture.Create<SampleComparableChildObject<EnumSmall>>()
                };

                var expected = one.Property.CompareTo(other.Property).Normalize();
                var actual = comparer.Compare(one, other).Normalize();

                actual.Should().Be(expected);
            }

            SampleComparableChildObject<EnumSmall>.UsedCompareTo.Should().BeTrue();
        }

        private static void Test(Type comparableGenericType, string comparerName, bool makeNullable)
        {
            var types = makeNullable ? SampleTypes.NullableTypes : SampleTypes.Types;
            Parallel.ForEach(
                types,
                item => {
                    var (type, referenceComparer) = item;
                    var objectType = type;
                    var itemComparer = referenceComparer;

                    var comparableType = comparableGenericType.MakeGenericType(objectType);
                    if (itemComparer != null) {
                        comparableType
                            .GetField(comparerName, BindingFlags.Public | BindingFlags.Static)
                            .SetValue(null, itemComparer);
                    }

                    new GenericTests().GenericTest(comparableType, null, false, Constants.SmallCount);
                });
        }
    }
}
