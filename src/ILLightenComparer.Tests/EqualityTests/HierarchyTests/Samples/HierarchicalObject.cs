using ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples.Nested;
using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.EqualityTests.HierarchyTests.Samples
{
    public sealed class HierarchicalObject
    {
        public ComparableBaseObject<EnumSmall> ComparableField;
        public SealedNestedObject NestedField;
        public NestedStruct? NestedNullableStructField;
        public NestedStruct NestedStructField;

        public SealedNestedObject FirstProperty { get; set; }
        public NestedStruct? NestedNullableStructProperty { get; set; }
        public NestedStruct NestedStructProperty { get; set; }
        public SealedNestedObject SecondProperty { get; set; }
        public int Value { get; set; }
    }
}
