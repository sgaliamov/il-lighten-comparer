using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.StringTests
{
    public class ClassWithStringTests : BaseComparerTests<StringSampleObject>
    {
        protected override IComparer<StringSampleObject> ReferenceComparer { get; } =
            StringSampleObject.Comparer;
    }
}
