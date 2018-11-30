using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.IntegralTests
{
    public class IntegralPropertiesClassTests : BaseComparerTests<IntegralPropertiesSampleObject>
    {
        protected override IComparer<IntegralPropertiesSampleObject> ReferenceComparer { get; } =
            IntegralPropertiesSampleObject.Comparer;
    }
}
