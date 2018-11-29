using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests
{
    public sealed class HierarchyTests : BaseComparerTests<ContainerObject>
    {
        public HierarchyTests()
        {
            ComparersBuilder.For<NestedObject>()
                            .DefineConfiguration(new ComparerSettings
                            {
                                MembersOrder = new[]
                                {
                                    nameof(NestedObject.DeepNestedField),
                                    nameof(NestedObject.DeepNestedProperty)
                                }
                            })
                            .For<ContainerObject>()
                            .DefineConfiguration(new ComparerSettings
                            {
                                MembersOrder = new[]
                                {
                                    nameof(ContainerObject.Comparable),
                                    nameof(ContainerObject.Value)
                                }
                            });
        }
        protected override IComparer<ContainerObject> ReferenceComparer { get; } = ContainerObject.Comparer;
    }
}
