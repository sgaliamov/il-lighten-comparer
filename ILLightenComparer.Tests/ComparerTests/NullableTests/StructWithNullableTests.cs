using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.NullableTests
{
    public class StructWithNullableTests : BaseComparerTests<NullableSampleStruct>
    {
        protected override IComparer<NullableSampleStruct> ReferenceComparer { get; } =
            NullableSampleStruct.Comparer;
    }
}
