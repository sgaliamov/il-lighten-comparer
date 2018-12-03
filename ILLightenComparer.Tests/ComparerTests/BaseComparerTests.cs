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
        public void Comparison_Of_Null_With_Object_Produces_Negative_Value()
        {
            var obj = Fixture.Create<T>();

            BasicComparer.Compare(null, obj).Should().BeNegative();
            if (typeof(T).IsClass)
            {
                TypedComparer.Compare(default, obj).Should().BeNegative();
            }
        }

        [Fact]
        public void Comparison_Of_Object_With_Null_Produces_Positive_Value()
        {
            var obj = Fixture.Create<T>();

            BasicComparer.Compare(obj, null).Should().BePositive();
            if (typeof(T).IsClass)
            {
                TypedComparer.Compare(obj, default).Should().BePositive();
            }
        }

        [Fact]
        public void Comparison_With_Itself_Produces_0()
        {
            var obj = Fixture.Create<T>();

            BasicComparer.Compare(obj, obj).Should().Be(0);
            TypedComparer.Compare(obj, obj).Should().Be(0);
        }

        [Fact]
        public void Mutate_Class_Members_And_Test_Comparision()
        {
            if (typeof(T).IsValueType)
            {
                return;
            }

            for (var i = 0; i < 10; i++)
            {
                var original = Fixture.Create<T>();

                foreach (var mutant in Fixture.CreateMutants(original))
                {
                    ReferenceComparer.Compare(mutant, original).Should().NotBe(0);
                    BasicComparer.Compare(mutant, original).Should().NotBe(0);
                    TypedComparer.Compare(mutant, original).Should().NotBe(0);
                }
            }
        }

        [Fact]
        public void Sorting_Must_Work_The_Same_As_For_Reference_Comparer()
        {
            var original = Fixture.CreateMany<T>(Count).ToArray();
            var copy0 = original.DeepClone();
            var copy1 = original.DeepClone();
            var copy2 = original.DeepClone();

            Array.Sort(copy0, ReferenceComparer);
            Array.Sort(copy1, BasicComparer);
            Array.Sort(copy2, TypedComparer);

            copy0.ShouldBeSameOrder(copy1);
            copy0.ShouldBeSameOrder(copy2);
        }

        protected readonly Fixture Fixture = FixtureBuilder.GetInstance();

        protected IComparer BasicComparer => ComparersBuilder.GetComparer(typeof(T));

        protected IContextBuilder ComparersBuilder =>
            _comparersBuilder
            ?? (_comparersBuilder = new ComparersBuilder()
                .DefineDefaultConfiguration(new ComparerSettings
                {
                    IncludeFields = true
                }));

        protected abstract IComparer<T> ReferenceComparer { get; }

        protected IComparer<T> TypedComparer => ComparersBuilder.GetComparer<T>();

        private const int Count = 10000;

        private IContextBuilder _comparersBuilder;
    }
}
