using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Force.DeepCloner;
using ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples;
using ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples.Nested;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests
{
    public class MultiThreadingTests
    {
        public MultiThreadingTests()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public void Generate_Comparer_For_Not_Sealed_Member_In_Parallel_Still_Works()
        {
            var comparer = new ComparersBuilder().For<AbstractMembers>().GetComparer();

            void Warmup(AbstractMembers obj) => comparer.Compare(obj, obj.DeepClone()).Should().Be(0);

            var one = new AbstractMembers
            {
                NotSealedProperty = _fixture.Create<BaseNestedObject>()
            };
            Warmup(one);

            one.NotSealedProperty = _fixture.Create<AnotherNestedObject>();
            var other = new AbstractMembers
            {
                NotSealedProperty = _fixture.Create<AnotherNestedObject>()
            };

            var expected = AbstractMembers.Comparer.Compare(one, other);

            void Test(int i)
            {
                var actual = comparer.Compare(one, other);
                actual.Should().Be(expected);
            }

            Parallel.For(0, Environment.ProcessorCount, Test);
        }

        private readonly Fixture _fixture;
    }
}
