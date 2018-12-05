using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Force.DeepCloner;
using ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples;
using ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples.Nested;
using ILLightenComparer.Tests.Utilities;
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
            var one = new AbstractMembers
            {
                NotSealedProperty = _fixture.Create<AnotherNestedObject>()
            };

            void Run(int _)
            {
                var comparer = CreateComparer();

                var other = new AbstractMembers
                {
                    NotSealedProperty = _fixture.Create<AnotherNestedObject>()
                };

                var expected = AbstractMembers.Comparer.Compare(one, other).Normalize();

                void Test(int i)
                {
                    var actual = comparer.Compare(one, other).Normalize();
                    actual.Should().Be(expected);
                }

                Parallel.For(0, Environment.ProcessorCount, Test);
            } 

            Parallel.For(0, Environment.ProcessorCount, Run);
        }

        private static IComparer<AbstractMembers> CreateComparer() =>
            new ComparersBuilder()
                .For<AnotherNestedObject>()
                .DefineConfiguration(new ComparerSettings
                {
                    MembersOrder = new[]
                    {
                        nameof(AnotherNestedObject.Value),
                        nameof(AnotherNestedObject.Key),
                        nameof(AnotherNestedObject.Text)
                    }
                })
                .For<AbstractMembers>()
                .GetComparer();

        private readonly Fixture _fixture;
    }
}
