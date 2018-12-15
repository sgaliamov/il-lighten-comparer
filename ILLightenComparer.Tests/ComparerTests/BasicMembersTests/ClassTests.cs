﻿using System.Collections.Generic;
using ILLightenComparer.Tests.ComparerTests.BasicMembersTests.Samples;

namespace ILLightenComparer.Tests.ComparerTests.BasicMembersTests
{
    public class ClassTests : BaseComparerTests<SampleObject>
    {
        protected override IComparer<SampleObject> ReferenceComparer { get; } =
            SampleObject.Comparer;
    }
}
