namespace ILLightenComparer.Tests.EqualityTests.HierarchyTests.Samples.Nested
{
    public sealed class SealedNestedObject : BaseNestedObject
    {
        public DeepNestedObject DeepNestedField;
        public DeepNestedObject DeepNestedProperty { get; set; }
    }
}
