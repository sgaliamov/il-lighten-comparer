using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.IntegralTests
{
    public class ClassWithIntegralTests : BaseComparerTests<IntegralSampleObject>
    {
        protected override IComparer<IntegralSampleObject> ReferenceComparer { get; } =
            IntegralSampleObject.Comparer;
    }
}
