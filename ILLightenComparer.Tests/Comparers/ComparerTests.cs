using System;
using System.Collections;
using System.Collections.Generic;
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
        public void Compare_Structs_By_NotGeneric_Comparer()
        {
            var comparer = new ComparersBuilder().CreateComparer(typeof(SampleStruct));

            var original = _fixture.CreateMany<SampleStruct>(Count).ToArray();
            var copy = original.DeepClone();

            Array.Sort(copy, SampleStruct.Comparer);
            Array.Sort(original, comparer);

            Compare(original, copy);
        }

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
            var original = _fixture.CreateMany<TestObject>(Count).ToArray();
            var copy = original.DeepClone();

            Array.Sort(copy, TestObject.TestObjectComparer);
            Array.Sort(original, _target);

            Compare(original, copy);
        }

        [Fact]
        public void Comparison_Wrong_Type_Throw_Exception()
        {
            Assert.Throws<InvalidCastException>(() => _target.Compare(new SampleObject(), new SampleObject()));
        }

        private const int Count = 100;

        private static void Compare<T>(IEnumerable<T> one, IEnumerable<T> other)
        {
            using (var enumeratorOne = one.GetEnumerator())
            using (var enumeratorOther = other.GetEnumerator())
            {
                while (enumeratorOne.MoveNext() && enumeratorOther.MoveNext())
                {
                    var oneCurrent = enumeratorOne.Current;
                    var otherCurrent = enumeratorOther.Current;

                    oneCurrent
                        .Should()
                        .BeEquivalentTo(
                            otherCurrent,
                            $"object \n{{\n\t{oneCurrent}\n}} must be equal to\n{{\n\t{otherCurrent}\n}}");
                }

                enumeratorOne.MoveNext().Should().BeFalse();
                enumeratorOther.MoveNext().Should().BeFalse();
            }
        }

        private readonly IComparer _target = new ComparersBuilder().CreateComparer(typeof(TestObject));
        private readonly Fixture _fixture = FixtureBuilder.GetInstance();
    }
}
