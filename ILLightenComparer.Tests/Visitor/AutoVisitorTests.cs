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
        public void Acceptor_Is_Defined_As_Object()
        {
            var sampleObject = _fixture.Create<SampleObject>();
            var acceptorObject = (object)sampleObject;

            _target.Accept(acceptorObject, new SampleVisitor(), 1)
                   .Should()
                   .Be(sampleObject.KeyField);
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

            _target.Accept(sampleObject, new SampleVisitor(), 1)
                   .Should()
                   .Be(sampleObject.KeyField);

            _target.Accept(sampleParentObject, new SampleVisitor(), 1)
                   .Should()
                   .Be(sampleParentObject.KeyField);
        }

        [Fact]
        public void Box_Struct_Acceptor()
        {
            var sampleStruct = _fixture.Create<SampleStruct>();
            var boxedStruct = (object)sampleStruct;
            var interfaceStruct = (ISample)sampleStruct;

            _target.Accept(interfaceStruct, new SampleVisitor(), 1)
                   .Should()
                   .Be(sampleStruct.KeyField + 1);

            _target.Accept(sampleStruct, new SampleVisitor(), 1)
                   .Should()
                   .Be(sampleStruct.KeyField + 1);

            _target.Accept(boxedStruct, new SampleVisitor(), 1)
                   .Should()
                   .Be(sampleStruct.KeyField + 1);
        }

        [Fact]
        public void Box_Struct_Visitor()
        {
            var sampleStruct = _fixture.Create<SampleStruct>();
            var boxedStruct = (object)sampleStruct;

            IVisitor<SampleStruct, int> visitor = new SampleStructVisitor();

            _target.Accept(boxedStruct, visitor, 1)
                   .Should()
                   .Be(sampleStruct.KeyField);

            _target.Accept(boxedStruct, (object)visitor, 1)
                   .Should()
                   .Be(sampleStruct.KeyField);
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

        [Fact]
        public void Visitor_Used_As_Interface()
        {
            var acceptor = _fixture.Create<SampleObject>();

            IVisitor<SampleObject, int> visitor = new SampleVisitor();

            var actual = _target.Accept(acceptor, visitor, 1);

            actual.Should().Be(acceptor.KeyField);
        }

        private readonly AutoVisitor _target = new AutoVisitor(true, nameof(SampleVisitor.Do));
        private readonly Fixture _fixture;
    }
}
