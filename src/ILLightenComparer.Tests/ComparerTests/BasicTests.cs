using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests
{
    public sealed class BasicTests
    {
        [Fact]
        public void Empty_object_should_be_equal()
        {
            var comparer = new ComparerBuilder().GetComparer<DummyObject>();

            var actual = comparer.Compare(new DummyObject(), new DummyObject());

            actual.Should().Be(0);
        }

        [Fact]
        public void Empty_structs_should_be_equal()
        {
            var comparer = new ComparerBuilder().GetComparer<DummyStruct>();

            var actual = comparer.Compare(new DummyStruct(), new DummyStruct());

            actual.Should().Be(0);
        }

        [Fact]
        public void Empty_nullable_structs_should_be_equal()
        {
            var comparer = new ComparerBuilder().GetComparer<DummyStruct?>();

            var fixture = FixtureBuilder.GetInstance();

            fixture.Create<DummyStruct>();

            var actual = comparer.Compare(new DummyStruct(), new DummyStruct());

            actual.Should().Be(0);
        }

        [Fact]
        public void Enumerables_are_not_equal()
        {
            var x = new List<int>(new[] { 1, 2, 3 });
            var y = new List<int>(new[] { 2, 3, 1 });

            var comparer = new ComparerBuilder().GetComparer<List<int>>();

            var result = comparer.Compare(x, y);

            result.Should().Be(-1);
        }
    }
}
