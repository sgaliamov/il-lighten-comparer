using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.IntegralTests
{
    public class StructWithIntegralTests : BaseComparerTests<IntegralSampleStruct>
    {
        protected override IComparer<IntegralSampleStruct> ReferenceComparer { get; } =
            IntegralSampleStruct.Comparer;
    }
}
