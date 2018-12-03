using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.BasicMembersTests
{
    public class ClassTests : BaseComparerTests<SampleObject>
    {
        protected override IComparer<SampleObject> ReferenceComparer { get; } =
            SampleObject.Comparer;
    }
}
