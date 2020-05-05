using System.Diagnostics.CodeAnalysis;

namespace ILLightenComparer.Tests.EqualityTests.HierarchyTests.Samples.Nested
{
    [SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "<Pending>")]
    public struct NestedStruct
    {
        public ulong Property { get; set; }
        public ulong? NullableProperty { get; set; }
    }
}
