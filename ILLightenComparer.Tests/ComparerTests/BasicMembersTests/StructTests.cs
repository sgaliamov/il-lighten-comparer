using System.Collections.Generic;
using ILLightenComparer.Tests.ComparerTests.BasicMembersTests.Samples;

namespace ILLightenComparer.Tests.ComparerTests.BasicMembersTests
{
    public class StructTests : BaseComparerTests<SampleStruct>
    {
        protected override IComparer<SampleStruct> ReferenceComparer { get; } =
            SampleStruct.Comparer;
    }
}
