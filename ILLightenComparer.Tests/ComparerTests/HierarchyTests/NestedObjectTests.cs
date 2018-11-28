using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests
{
    public sealed class NestedObjectTests : BaseComparerTests<ContainerObject>
    {
        protected override IComparer<ContainerObject> ReferenceComparer { get; } = ContainerObject.Comparer;
    }
}
