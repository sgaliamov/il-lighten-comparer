using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Force.DeepCloner;
using ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests
{
    public class AbstractMembersTests
    {
        [Fact]
        public void AbstractProperty_Comparison()
        {
            Test(x => new AbstractProperties
            {
                AbstractProperty = x
            });
        }

        [Fact]
        public void InterfaceProperty_Comparison()
        {
            Test(x => new AbstractProperties
            {
                InterfaceProperty = x
            });
        }

        [Fact]
        public void NotSealedProperty_Comparison()
        {
            Test(x => new AbstractProperties
            {
                NotSealedProperty = x
            });
        }

        [Fact]
        public void ObjectProperty_Comparison()
        {
            Test(x => new AbstractProperties
            {
                ObjectProperty = x
            });
        }

        private void Test(Func<NestedObject, AbstractProperties> selector)
        {
            var original = _fixture
                           .Build<NestedObject>()
                           .Without(x => x.DeepNestedField)
                           .Without(x => x.DeepNestedProperty)
                           .CreateMany(1000)
                           .Select(selector)
                           .ToArray();

            var clone = original.DeepClone();

            Array.Sort(original, AbstractProperties.Comparer);
            Array.Sort(clone, _comparer);

            original.ShouldBeSameOrder(clone);
        }

        private readonly Fixture _fixture = FixtureBuilder.GetInstance();

        private readonly IComparer<AbstractProperties> _comparer =
            new ComparersBuilder()
                .DefineDefaultConfiguration(new ComparerSettings
                {
                    IncludeFields = true
                })
                .For<NestedObject>()
                .DefineConfiguration(new ComparerSettings
                {
                    IgnoredMembers = new[]
                    {
                        nameof(NestedObject.DeepNestedField),
                        nameof(NestedObject.DeepNestedProperty)
                    },
                    MembersOrder = new[]
                    {
                        nameof(NestedObject.Key),
                        nameof(NestedObject.Text)
                    }
                })
                .For<AbstractProperties>()
                .GetComparer();
    }
}
