using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Force.DeepCloner;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests
{
    public abstract class BaseComparerTests<T>
    {
        [Fact]
        public void Comparison_With_Itself_Produces_0()
        {
            var obj = Fixture.Create<T>();

            BasicComparer.Compare(obj, obj).Should().Be(0);
            TypedComparer.Compare(obj, obj).Should().Be(0);
        }

        [Fact]
        public void Sorting_Must_Work_The_Same_As_For_Reference_Comparer()
        {
            var original = Fixture.CreateMany<T>(Count).ToArray();
            var copy1 = original.DeepClone();
            var copy2 = original.DeepClone();

            Array.Sort(original, ReferenceComparer);
            Array.Sort(copy1, BasicComparer);
            Array.Sort(copy2, TypedComparer);

            Compare(original, copy1);
            Compare(original, copy2);
        }

        private const int Count = 10000;

        private static void Compare(IEnumerable<T> one, IEnumerable<T> other)
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

        protected abstract IComparer<T> ReferenceComparer { get; }
        protected readonly Fixture Fixture = FixtureBuilder.GetInstance();
        protected IComparer BasicComparer => ComparersBuilder.CreateComparer(typeof(T));
        protected IComparer<T> TypedComparer => ComparersBuilder.CreateComparer<T>();

        protected readonly ComparersBuilder ComparersBuilder =
            new ComparersBuilder()
                .SetConfiguration(
                    new CompareConfiguration
                    {
                        IncludeFields = true
                    });
    }
}
