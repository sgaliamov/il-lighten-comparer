using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.FlattenHierarchy
{
    public class FlattenHierarchyTests : BaseComparerTests<ChildObject>
    {
        protected override CompareConfiguration CompareConfiguration { get; } = new CompareConfiguration
        {
            IncludeFields = true,
            MembersOrder = new[]
            {
                "Field",
                "Property"
            }
        };

        protected override IComparer<ChildObject> ReferenceComparer { get; } =
            ChildObject.ChildObjectComparer;
    }
}
