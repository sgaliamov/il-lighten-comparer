namespace ILLightenComparer.Tests.EqualityTests.CycleTests.Samples
{
    public sealed class CycledStructObject
    {
        public CycledStruct? FirstStruct;
        public string TextField;
        public CycledStruct SecondStruct { get; set; }
    }
}
