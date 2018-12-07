using System;
using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.ComparerTests.ComparableTests.Samples
{
    public struct ComparableStruct : IComparable<ComparableStruct>
    {
        public EnumBig Property { get; set; }

        public int CompareTo(ComparableStruct other) => Property.CompareTo(other.Property);
    }
}
