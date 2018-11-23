using System;
using AutoFixture;
using FluentAssertions;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Visitor;
using Xunit;

namespace ILLightenComparer.Tests.Visitor
{
    public class AutoVisitorTests
    {
        public AutoVisitorTests()
        {
            _fixture = new Fixture();
            _fixture.Customize(new SupportMutableValueTypesCustomization());
        }

        [Fact]
        public void Acceptors_Are_Defined_As_ParentClass()
        {
            var sampleObject = _fixture.Create<SampleObject>();
            var acceptorObject = (SampleParentObject)sampleObject;

            var sampleParentObject = _fixture.Create<SampleParentObject>();

            _target.Accept(acceptorObject, new SampleVisitor(), 1)
                   .Should()
                   .Be(sampleObject.KeyField);

            _target.Accept(sampleParentObject, new SampleVisitor(), 1)
                   .Should()
                   .Be(sampleParentObject.KeyField);
        }

        [Fact]
        public void Acceptors_Are_Defined_As_Object()
        {
            var sampleObject = _fixture.Create<SampleObject>();
            var acceptorObject = (object)sampleObject;

            var sampleStruct = _fixture.Create<SampleStruct>();
            var acceptorStruct = (object)sampleStruct;

            _target.Accept(acceptorObject, new SampleVisitor(), 1)
                   .Should()
                   .Be(sampleObject.KeyField);

            _target.Accept(acceptorStruct, new SampleVisitor(), 1)
                   .Should()
                   .Be(sampleStruct.KeyField + 1);
        }

        [Fact]
        public void Class_Acceptor_Accepts_Visitor_And_Mutate_State()
        {
            var acceptor = _fixture.Create<SampleObject>();

            var actual = _target.Accept(acceptor, new SampleVisitor(), 1);

            actual.Should().Be(acceptor.KeyField);
        }

        [Fact]
        public void Struct_Acceptor_Accepts_Visitor_And_Copy_Struct()
        {
            var acceptor = _fixture.Create<SampleStruct>();

            var actual = _target.Accept(acceptor, new SampleVisitor(), 1);

            actual.Should().Be(acceptor.KeyField + 1);
        }

        [Fact]
        public void Unknown_Acceptor_Throws_NotSupportedException()
        {
            var acceptor = _fixture.Create<TestObject>();

            Assert.Throws<NotSupportedException>(() => _target.Accept(acceptor, new SampleVisitor(), 1));
        }

        private readonly AutoVisitor _target = new AutoVisitor(true, "TestVisit");
        private readonly Fixture _fixture;
    }
}
