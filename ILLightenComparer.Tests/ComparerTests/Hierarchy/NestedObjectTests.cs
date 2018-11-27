using System.Collections.Generic;
using ILLightenComparer.Tests.ComparerTests.ComparableTests;

namespace ILLightenComparer.Tests.ComparerTests.Hierarchy
{
    public sealed class NestedObjectTests : BaseComparerTests<ContainerObject>
    {
        protected override IComparer<ContainerObject> ReferenceComparer { get; } = ContainerObject.Comparer;
    }
}
