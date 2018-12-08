using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.BasicMembersTests
{
    public class StructTests : BaseComparerTests<SampleStruct>
    {
        protected override IComparer<SampleStruct> ReferenceComparer { get; } =
            SampleStruct.Comparer;
    }
}
