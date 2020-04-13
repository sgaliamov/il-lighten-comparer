using FluentAssertions;
using ILLightenComparer.Tests.Samples;
using Xunit;

namespace ILLightenComparer.Tests.EqualityTests
{
    public sealed class SampleTypesTests
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

            equality.Should().BeTrue();
            hashX.Should().Be(hashY);
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

            equality.Should().BeTrue();
            hashX.Should().Be(hashY);
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

            equality.Should().BeTrue();
            hashX.Should().Be(hashY);
        }
    }
}
