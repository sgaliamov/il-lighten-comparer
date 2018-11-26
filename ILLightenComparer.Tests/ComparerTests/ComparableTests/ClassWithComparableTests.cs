using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.ComparableTests
{
    public class ClassWithComparableTests : BaseComparerTests<ComparableSampleObject>
    {
        protected override IComparer<ComparableSampleObject> ReferenceComparer { get; } =
            ComparableSampleObject.Comparer;
    }
}
