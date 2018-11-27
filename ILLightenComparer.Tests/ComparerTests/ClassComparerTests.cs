using System;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using ILLightenComparer.Tests.Samples;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests
{
    public sealed class ClassComparerTests : BaseComparerTests<TestObject>
    {
        [Fact]
        public void Comparison_Of_Null_With_Object_Produces_Negative_Value()
        {
            var obj = Fixture.Create<TestObject>();

            BasicComparer.Compare(default(TestObject), obj).Should().BeLessThan(0);
            TypedComparer.Compare(default, obj).Should().BeLessThan(0);
        }

        [Fact]
        public void Comparison_Of_Object_With_Null_Produces_Positive_Value()
        {
            var obj = Fixture.Create<TestObject>();

            BasicComparer.Compare(obj, default(TestObject)).Should().BeGreaterThan(0);
            TypedComparer.Compare(obj, default).Should().BeGreaterThan(0);
        }

        [Fact]
        public void Comparison_Wrong_Type_Throw_Exception()
        {
            Assert.Throws<InvalidCastException>(() => BasicComparer.Compare(new DummyObject(), new DummyObject()));
        }

        protected override IComparer<TestObject> ReferenceComparer { get; } = TestObject.TestObjectComparer;
    }
}
