using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.NullableTests
{
    public class ClassWithNullableTests : BaseComparerTests<NullableSampleObject>
    {
        protected override IComparer<NullableSampleObject> ReferenceComparer { get; } =
            NullableSampleObject.Comparer;
    }
}
