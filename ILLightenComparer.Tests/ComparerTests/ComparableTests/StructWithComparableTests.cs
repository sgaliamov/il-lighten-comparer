using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.ComparableTests
{
    public class StructWithComparableTests : BaseComparerTests<ComparableSampleStruct>
    {
        protected override IComparer<ComparableSampleStruct> ReferenceComparer { get; } =
            ComparableSampleStruct.Comparer;
    }
}
