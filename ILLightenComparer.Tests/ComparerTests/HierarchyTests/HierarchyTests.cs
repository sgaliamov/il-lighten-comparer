using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests
{
    public sealed class HierarchyTests : BaseComparerTests<ContainerObject>
    {
        public HierarchyTests()
        {
            ComparersBuilder.For<NestedObject>()
                            .SetConfiguration(new CompareConfiguration
                            {
                                MembersOrder = 
                            })
        }
        protected override IComparer<ContainerObject> ReferenceComparer { get; } = ContainerObject.Comparer;
    }
}
