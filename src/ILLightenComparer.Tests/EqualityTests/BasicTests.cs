using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using ILLightenComparer.Tests.EqualityComparers;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.EqualityTests
{
    public sealed class BasicTests
    {
        private readonly IFixture _fixture = FixtureBuilder.GetInstance();

        [Fact]
        public void Empty_object_should_be_equal()
        {
            var x = new DummyObject();
            var y = new DummyObject();

            var comparer = new ComparerBuilder().GetEqualityComparer<DummyObject>();
            var equality = comparer.Equals(x, y);
            var hashX = comparer.GetHashCode(x);
            var hashY = comparer.GetHashCode(y);

            using (new AssertionScope()) {
                equality.Should().BeTrue();
                comparer.Equals(null, null).Should().BeTrue();
                hashX.Should().Be(hashY);
            }
        }

        [Fact]
        public void Empty_structs_should_be_equal()
        {
            var x = new DummyStruct();
            var y = new DummyStruct();

            var comparer = new ComparerBuilder().GetEqualityComparer<DummyStruct>();
            var equality = comparer.Equals(x, y);
            var hashX = comparer.GetHashCode(x);
            var hashY = comparer.GetHashCode(y);

            using (new AssertionScope()) {
                equality.Should().BeTrue();
                hashX.Should().Be(hashY);
            }
        }

        [Fact]
        public void Compare_identical_arrays()
        {
            var x = new[] { 1, 2, 3 };
            var y = new[] { 1, 2, 3 };

            var comparer = new ComparerBuilder().GetEqualityComparer<int[]>();
            var equality = comparer.Equals(x, y);
            var hashX = comparer.GetHashCode(x);
            var hashY = comparer.GetHashCode(y);

            using (new AssertionScope()) {
                equality.Should().BeTrue();
                comparer.Equals(null, null).Should().BeTrue();
                hashX.Should().Be(hashY);
            }
        }

        [Fact]
        public void Compare_with_null_array_should_be_not_equal()
        {
            var x = new[] { 1, 2, 3 };

            var comparer = new ComparerBuilder().GetEqualityComparer<int[]>();
            var equality = comparer.Equals(null, x);
            var hashY = comparer.GetHashCode(null);

            using (new AssertionScope()) {
                equality.Should().BeFalse();
                hashY.Should().Be(0);
            }
        }

        [Fact]
        public void Compare_with_null_object_should_be_not_equal()
        {
            var x = _fixture.Create<SampleObject<int[]>>();

            var comparer = new ComparerBuilder().GetEqualityComparer<SampleObject<int[]>>();
            var equality = comparer.Equals(x, null);
            var hashY = comparer.GetHashCode(null);

            using (new AssertionScope()) {
                equality.Should().BeFalse();
                hashY.Should().Be(0);
            }
        }

        [Fact]
        public void Compare_with_null_members_should_be_not_equal()
        {
            var x = _fixture.Create<SampleObject<int[]>>();
            var y = new SampleObject<int[]>();
            var expectedCustomHash = HashCodeCombiner.Combine(0, 0);

            var comparer = new ComparerBuilder().GetEqualityComparer<SampleObject<int[]>>();
            var equality = comparer.Equals(x, y);
            var hashY = comparer.GetHashCode(y);

            using (new AssertionScope()) {
                comparer.Equals(x, x).Should().BeTrue();
                equality.Should().Be(x.Field is null && x.Property is null);
                hashY.Should().Be(expectedCustomHash);
            }
        }

        [Fact]
        public void Equality_on_nullable_structs_works()
        {
            var x = _fixture.Create<SampleStruct<EnumBig>?>();
            var y = _fixture.Create<SampleStruct<EnumBig>?>();

            var referenceComparer = new NullableEqualityComparer<SampleStruct<EnumBig>>();
            var expectedHashX = referenceComparer.GetHashCode(x);
            var expectedHashY = referenceComparer.GetHashCode(y);
            var expectedEquals = referenceComparer.Equals(x, y);

            var comparer = new ComparerBuilder().GetEqualityComparer<SampleStruct<EnumBig>?>();

            var hashX = comparer.GetHashCode(x);
            var hashY = comparer.GetHashCode(y);
            var equals = comparer.Equals(x, y);

            using (new AssertionScope()) {
                comparer.Equals(x, x).Should().BeTrue();
                comparer.Equals(null, null).Should().BeTrue();
                equals.Should().Be(expectedEquals);
                hashX.Should().Be(expectedHashX);
                hashY.Should().Be(expectedHashY);
            }
        }

        [Fact]
        public void Equality_on_nullable_arguments_works()
        {
            var x = _fixture.Create<EnumBig?>();
            var y = _fixture.Create<EnumBig?>();

            var referenceComparer = new NullableEqualityComparer<EnumBig>();
            var expectedHashX = referenceComparer.GetHashCode(x);
            var expectedHashY = referenceComparer.GetHashCode(y);
            var expectedEquals = referenceComparer.Equals(x, y);

            var comparer = new ComparerBuilder().GetEqualityComparer<EnumBig?>();

            var hashX = comparer.GetHashCode(x);
            var hashY = comparer.GetHashCode(y);
            var equals = comparer.Equals(x, y);

            using (new AssertionScope()) {
                comparer.Equals(x, x).Should().BeTrue();
                comparer.Equals(null, null).Should().BeTrue();
                equals.Should().Be(expectedEquals);
                hashX.Should().Be(expectedHashX);
                hashY.Should().Be(expectedHashY);
            }
        }

        [Fact]
        public void Equality_on_array_of_nullables_works()
        {
            var x = new EnumSmall?[] { EnumSmall.First, null };
            var y = new EnumSmall?[] { EnumSmall.First, EnumSmall.One };

            var referenceComparer = new CollectionEqualityComparer<EnumSmall?>(new NullableEqualityComparer<EnumSmall>());
            var expectedHashX = referenceComparer.GetHashCode(x);
            var expectedHashY = referenceComparer.GetHashCode(y);
            var expectedEquals = referenceComparer.Equals(x, y);

            var comparer = new ComparerBuilder().GetEqualityComparer<EnumSmall?[]>();

            var hashX = comparer.GetHashCode(x);
            var hashY = comparer.GetHashCode(y);
            var equals = comparer.Equals(x, y);

            using (new AssertionScope()) {
                comparer.Equals(x, x).Should().BeTrue();
                comparer.Equals(null, null).Should().BeTrue();
                equals.Should().Be(expectedEquals);
                hashX.Should().Be(expectedHashX);
                hashY.Should().Be(expectedHashY);
            }
        }

        [Fact]
        public void Equality_on_member_array_of_nullables_works()
        {
            var x = _fixture.Create<SampleObject<EnumSmall?[]>>();
            x.Field = x.Field?.RandomNulls().ToArray();
            x.Property = x.Property?.RandomNulls().ToArray();
            var y = _fixture.Create<SampleObject<EnumSmall?[]>>();
            y.Field = y.Field?.RandomNulls().ToArray();
            y.Property = y.Property?.RandomNulls().ToArray();

            var referenceComparer = new SampleObjectEqualityComparer<EnumSmall?[]>(new CollectionEqualityComparer<EnumSmall?>());
            var expectedHashX = referenceComparer.GetHashCode(x);
            var expectedHashY = referenceComparer.GetHashCode(y);
            var expectedEquals = referenceComparer.Equals(x, y);

            var comparer = new ComparerBuilder().GetEqualityComparer<SampleObject<EnumSmall?[]>>();

            var hashX = comparer.GetHashCode(x);
            var hashY = comparer.GetHashCode(y);
            var equals = comparer.Equals(x, y);

            using (new AssertionScope()) {
                comparer.Equals(x, x).Should().BeTrue();
                comparer.Equals(null, null).Should().BeTrue();
                equals.Should().Be(expectedEquals);
                hashX.Should().Be(expectedHashX, x.ToStringEx());
                hashY.Should().Be(expectedHashY, y.ToStringEx());
            }
        }

        [Fact]
        public void Enumerables_are_not_equal()
        {
            var x = new List<int>(new[] { 1, 2, 3 });
            var y = new List<int>(new[] { 2, 3, 1 });

            var comparer = new ComparerBuilder().GetEqualityComparer<IEnumerable<int>>();
            var equals = comparer.Equals(x, y);
            var hashX = comparer.GetHashCode(x);
            var hashY = comparer.GetHashCode(y);

            using (new AssertionScope()) {
                equals.Should().BeFalse();
                hashX.Should().NotBe(hashY);
            }
        }

        [Fact]
        public void Enumerable_structs_are_comparable()
        {
            var x = _fixture.Create<EnumerableStruct<int>>();
            var y = _fixture.Create<EnumerableStruct<int>>();

            var referenceComparer = new CollectionEqualityComparer<int>();
            var expectedHashX = referenceComparer.GetHashCode(x);
            var expectedHashY = referenceComparer.GetHashCode(y);
            var expectedEquals = referenceComparer.Equals(x, y);

            var comparer = new ComparerBuilder().GetEqualityComparer<EnumerableStruct<int>>();
            var equals = comparer.Equals(x, y);
            var hashX = comparer.GetHashCode(x);
            var hashY = comparer.GetHashCode(y);

            using (new AssertionScope()) {
                comparer.Equals(x, x).Should().BeTrue();
                comparer.Equals(default, default).Should().BeTrue();
                equals.Should().Be(expectedEquals);
                hashX.Should().Be(expectedHashX);
                hashY.Should().Be(expectedHashY);
            }
        }


        [Fact]
        public void Enumerable_structs_with_nullables_are_comparable()
        {
            var x = _fixture.Create<EnumerableStruct<SampleStruct<int?>?>>();
            var y = _fixture.Create<EnumerableStruct<SampleStruct<int?>?>>();

            var referenceComparer = new CollectionEqualityComparer<SampleStruct<int?>?>(new NullableEqualityComparer<SampleStruct<int?>>(new SampleStructEqualityComparer<int?>()));
            var expectedHashX = referenceComparer.GetHashCode(x);
            var expectedHashY = referenceComparer.GetHashCode(y);
            var expectedEquals = referenceComparer.Equals(x, y);

            var comparer = new ComparerBuilder().GetEqualityComparer<EnumerableStruct<SampleStruct<int?>?>>();
            var equals = comparer.Equals(x, y);
            var hashX = comparer.GetHashCode(x);
            var hashY = comparer.GetHashCode(y);

            using (new AssertionScope()) {
                comparer.Equals(x, x).Should().BeTrue();
                comparer.Equals(default, default).Should().BeTrue();
                equals.Should().Be(expectedEquals);
                hashX.Should().Be(expectedHashX);
                hashY.Should().Be(expectedHashY);
            }
        }

        [Fact]
        public void Null_enumerator_pass()
        {
            var x = new EnumerableStruct<int>(null);

            var comparer = new ComparerBuilder().GetEqualityComparer<EnumerableStruct<int>>();
            var equals = comparer.Equals(x, x);
            var hashX = comparer.GetHashCode(x);

            using (new AssertionScope()) {
                equals.Should().BeTrue();
                hashX.Should().Be(0);
            }
        }
    }
}
