using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.StringTests
{
    public class StructWithStringTests : BaseComparerTests<StringSampleStruct>
    {
        protected override IComparer<StringSampleStruct> ReferenceComparer { get; } =
            StringSampleStruct.Comparer;
    }
}
