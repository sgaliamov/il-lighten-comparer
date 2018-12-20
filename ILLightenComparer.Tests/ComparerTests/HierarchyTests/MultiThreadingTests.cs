using System;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples;
using ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples.Nested;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests
{
    public sealed class MultiThreadingTests
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

            Helper.Parallel(() =>
                {
                    var comparer = CreateComparer();

                    var other = new AbstractMembers
                    {
                        NotSealedProperty = _fixture.Create<AnotherNestedObject>()
                    };

                    var expected = AbstractMembers.Comparer.Compare(one, other).Normalize();

                    Helper.Parallel(() =>
                        {
                            var actual = comparer.Compare(one, other).Normalize();
                            actual.Should().Be(expected);
                        },
                        Environment.ProcessorCount);
                },
                Environment.ProcessorCount * 10);
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
