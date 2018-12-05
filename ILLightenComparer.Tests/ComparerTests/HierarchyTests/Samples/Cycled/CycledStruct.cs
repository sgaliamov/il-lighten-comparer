namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples.Cycled
{
    public sealed class CycledStructObject
    {
        public CycledStruct Value { get; set; } // todo: test with nullable
    }

    public struct CycledStruct
    {
        public CycledStructObject Object { get; set; }
    }
}
