using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.InheritanceTests
{
    public sealed class FlattenHierarchyTests : BaseComparerTests<ChildObject>
    {
        public FlattenHierarchyTests()
        {
            ComparersBuilder.For<ChildObject>()
                            .DefineConfiguration(new ComparerSettings
                            {
                                MembersOrder = new[]
                                {
                                    nameof(ChildObject.Field),
                                    nameof(ChildObject.Property)
                                }
                            });
        }

        protected override IComparer<ChildObject> ReferenceComparer { get; } = ChildObject.ChildObjectComparer;
    }
}
