using System;
using System.Reflection;
using System.Threading.Tasks;
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
        public void Compare_comparable_objects()
        {
            Test(typeof(ComparableBaseObject<>), nameof(ComparableBaseObject<object>.Comparer), false);
            Test(typeof(ComparableChildObject<>), nameof(ComparableChildObject<object>.ChildComparer), false);
            Test(typeof(ComparableBaseObject<>), nameof(ComparableBaseObject<object>.Comparer), true);
            Test(typeof(ComparableChildObject<>), nameof(ComparableChildObject<object>.ChildComparer), true);

            foreach (var item in SampleTypes.Types) {
                typeof(ComparableBaseObject<>)
                    .MakeGenericType(item.Key)
                    .GetField(nameof(ComparableBaseObject<object>.UsedCompareTo), BindingFlags.Public | BindingFlags.Static)
                    .GetValue(null)
                    .Should()
                    .Be(true);
            }
        }

        [Fact]
        public void Compare_comparable_structs()
        {
            Test(typeof(ComparableStruct<>), nameof(ComparableStruct<object>.Comparer), false);
            Test(typeof(ComparableStruct<>), nameof(ComparableStruct<object>.Comparer), true);
        }

        [Fact]
        public void Custom_comparable_implementation_should_return_negative_when_first_argument_isnull()
        {
            var one = new SampleObject<ComparableBaseObject<EnumSmall>> {
                Property = FixtureBuilder.GetInstance().Create<ComparableBaseObject<EnumSmall>>()
            };

            var other = one.DeepClone();
            one.Property = null;

            var comparer = new ComparerBuilder().GetComparer<SampleObject<ComparableBaseObject<EnumSmall>>>();

            comparer.Compare(one, other).Should().BeNegative();
        }

        [Fact]
        public void Replaced_comparable_object_is_compared_with_custom_implementation()
        {
            var comparer = new ComparerBuilder().GetComparer<SampleObject<ComparableBaseObject<EnumSmall>>>();
            var fixture = FixtureBuilder.GetInstance();

            var one = new SampleObject<ComparableBaseObject<EnumSmall>> {
                Property = fixture.Create<ComparableChildObject<EnumSmall>>()
            };
            comparer.Compare(one, one.DeepClone()).Should().Be(0);

            for (var i = 0; i < Constants.SmallCount; i++) {
                one.Property = fixture.Create<ComparableChildObject<EnumSmall>>();
                var other = new SampleObject<ComparableBaseObject<EnumSmall>> {
                    Property = fixture.Create<ComparableChildObject<EnumSmall>>()
                };

                var expected = one.Property.CompareTo(other.Property).Normalize();
                var actual = comparer.Compare(one, other).Normalize();

                actual.Should().Be(expected);
            }

            ComparableChildObject<EnumSmall>.UsedCompareTo.Should().BeTrue();
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
