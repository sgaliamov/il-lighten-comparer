namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples.Cycled
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
