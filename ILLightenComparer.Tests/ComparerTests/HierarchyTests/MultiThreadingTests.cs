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

        [Fact(Timeout = Constants.DefaultTimeout)]
        public void Generate_Comparer_For_Not_Sealed_Member_In_Parallel_Still_Works()
        {
            var one = new AbstractMembers
            {
                NotSealedProperty = _fixture.Create<AnotherNestedObject>()
            };

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
                        },
                        Environment.ProcessorCount);
                },
                Environment.ProcessorCount * 10);
        }

        private static void Parallel(ThreadStart action, int count)
        {
            var threads = Enumerable
                          .Range(0, count)
                          .Select(x => new Thread(action))
                          .ToArray();

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }

        private static IComparer<AbstractMembers> CreateComparer()
        {
            return new ComparersBuilder()
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
        }

        private readonly Fixture _fixture;
    }
}
