using System;
using System.Collections;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Force.DeepCloner;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.Comparers
{
    public sealed class ComparerTests
    {
        [Fact]
        public void Comparison_Of_Null_With_Object_Produces_Negative_Value()
        {
            var obj = _fixture.Create<TestObject>();

            var actual = _target.Compare(null, obj);

            actual.Should().BeLessThan(0);
        }

        [Fact]
        public void Comparison_Of_Object_With_Null_Produces_Positive_Value()
        {
            var obj = _fixture.Create<TestObject>();

            var actual = _target.Compare(obj, null);

            actual.Should().BeGreaterThan(0);
        }

        [Fact]
        public void Comparison_With_Itself_Produces_0()
        {
            var obj = _fixture.Create<TestObject>();

            var actual = _target.Compare(obj, obj);

            actual.Should().Be(0);
        }

        [Fact]
        public void Sorting_Must_Work_The_Same_As_For_Reference_Comparer()
        {
            const int count = 100;
            var original = _fixture.CreateMany<TestObject>(count).ToArray();
            var copy = original.DeepClone();

            Array.Sort(original, _target);
            Array.Sort(copy, TestObject.Comparer);

            for (var i = 0; i < count; i++)
            {
                original[i]
                    .Should()
                    .BeEquivalentTo(
                        copy[i],
                        $"object \n{{\n\t{original[i]}\n}} must be equal to\n{{\n\t{copy[i]}\n}}");
            }
        }

        private readonly IComparer _target = new ComparersBuilder().CreateComparer(typeof(TestObject));
        private readonly Fixture _fixture = FixtureBuilder.GetInstance();
    }
}
