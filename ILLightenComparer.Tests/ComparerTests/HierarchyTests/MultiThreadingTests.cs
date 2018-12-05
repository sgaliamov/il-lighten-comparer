using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoFixture;
using FluentAssertions;
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

            for (var j = 0; j < 10; j++)
            {
                Parallel(() =>
                {
                    var comparer = CreateComparer();

                    var other = new AbstractMembers
                    {
                        NotSealedProperty = _fixture.Create<AnotherNestedObject>()
                    };

                    var expected = AbstractMembers.Comparer.Compare(one, other).Normalize();

                    Parallel(() =>
                    {
                        var actual = comparer.Compare(one, other).Normalize();
                        actual.Should().Be(expected);
                    });
                });
            }
        }

        private static void Parallel(Action action)
        {
            var barrier = new Barrier(Environment.ProcessorCount + 1);

            Enumerable
                .Range(0, Environment.ProcessorCount)
                .Select(x => new Thread(() =>
                {
                    action();
                    barrier.SignalAndWait();
                }))
                .ToList()
                .ForEach(thread => thread.Start());

            barrier.SignalAndWait();
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
