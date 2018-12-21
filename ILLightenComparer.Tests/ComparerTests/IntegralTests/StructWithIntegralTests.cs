using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.IntegralTests
{
    public sealed class StructWithIntegralTests : BaseComparerTests<IntegralSampleStruct>
    {
        protected override IComparer<IntegralSampleStruct> ReferenceComparer { get; } =
            IntegralSampleStruct.Comparer;
    }
}
