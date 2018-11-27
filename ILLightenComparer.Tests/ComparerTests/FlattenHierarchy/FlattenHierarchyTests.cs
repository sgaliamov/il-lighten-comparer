using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.FlattenHierarchy
{
    public class FlattenHierarchyTests : BaseComparerTests<ChildObject>
    {
        protected override IComparer<ChildObject> ReferenceComparer { get; } =
            ChildObject.ChildObjectComparer;
    }
}
