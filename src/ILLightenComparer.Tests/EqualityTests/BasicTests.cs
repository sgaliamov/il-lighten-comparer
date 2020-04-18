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
                equality.Should().BeFalse();
                hashY.Should().Be(expectedCustomHash);
            }
        }

        [Fact]
        public void Equality_on_Nullable_structs_works()
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
            var equals = comparer.Equals(x);

            equals.Should().Be(expectedEquals);
            hashX.Should().Be(expectedHashX);
            hashY.Should().Be(expectedHashY);
        }

        [Fact]
        public void Equality_on_Nullable_arguments_works()
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
            var equals = comparer.Equals(x);

            equals.Should().Be(expectedEquals);
            hashX.Should().Be(expectedHashX);
            hashY.Should().Be(expectedHashY);
        }

        private readonly IFixture _fixture = FixtureBuilder.GetInstance();
    }
}
