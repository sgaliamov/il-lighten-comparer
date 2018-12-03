using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Force.DeepCloner;
using ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples;
using ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples.Nested;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests
{
    public class AbstractMembersTests
    {
        [Fact]
        public void AbstractProperty_Comparison()
        {
            Test(x => new AbstractMembers
            {
                AbstractProperty = x
            });
        }

        [Fact]
        public void InterfaceField_Comparison()
        {
            Test(x => new AbstractMembers
            {
                InterfaceField = x
            });
        }

        [Fact]
        public void NotSealedProperty_Comparison()
        {
            Test(x => new AbstractMembers
            {
                NotSealedProperty = x
            });
        }

        [Fact]
        public void ObjectField_Comparison()
        {
            Test(x => new AbstractMembers
            {
                ObjectField = x
            });
        }

        private void Test(Func<SealedNestedObject, AbstractMembers> selector)
        {
            var original = _fixture
                           .Build<SealedNestedObject>()
                           .Without(x => x.DeepNestedField)
                           .Without(x => x.DeepNestedProperty)
                           .CreateMany(1000)
                           .Select(selector)
                           .ToArray();

            var clone = original.DeepClone();

            Array.Sort(original, AbstractMembers.Comparer);
            Array.Sort(clone, _comparer);

            original.ShouldBeSameOrder(clone);
        }

        private readonly Fixture _fixture = FixtureBuilder.GetInstance();

        private readonly IComparer<AbstractMembers> _comparer =
            new ComparersBuilder()
                .DefineDefaultConfiguration(new ComparerSettings
                {
                    IncludeFields = true
                })
                .For<SealedNestedObject>()
                .DefineConfiguration(new ComparerSettings
                {
                    IgnoredMembers = new[]
                    {
                        nameof(SealedNestedObject.DeepNestedField),
                        nameof(SealedNestedObject.DeepNestedProperty)
                    },
                    MembersOrder = new[]
                    {
                        nameof(SealedNestedObject.Key),
                        nameof(SealedNestedObject.Text)
                    }
                })
                .For<AbstractMembers>()
                .GetComparer();
    }
}
