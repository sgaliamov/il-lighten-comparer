namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples.Cycle
{
    public sealed class ObjectWithCycledStruct
    {
        public CycledStruct Value { get; set; }
    }

    public struct CycledStruct
    {
        public ObjectWithCycledStruct Object { get; set; }
    }
}
