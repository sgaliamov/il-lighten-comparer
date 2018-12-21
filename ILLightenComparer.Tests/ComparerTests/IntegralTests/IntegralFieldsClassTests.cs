using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.IntegralTests
{
    public sealed class IntegralFieldsClassTests : BaseComparerTests<IntegralFieldsSampleObject>
    {
        protected override IComparer<IntegralFieldsSampleObject> ReferenceComparer { get; } =
            IntegralFieldsSampleObject.Comparer;
    }
}
