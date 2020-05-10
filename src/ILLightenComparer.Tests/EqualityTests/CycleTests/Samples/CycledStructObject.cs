using ILLightenComparer.Tests.Utilities;

namespace ILLightenComparer.Tests.EqualityTests.CycleTests.Samples
{
    public sealed class CycledStructObject
    {
        public readonly int Id;
        public CycledStruct? FirstStruct;
        public string TextField;
        public CycledStruct SecondStruct { get; set; }

        public CycledStructObject() => Id = this.GetObjectId();

        public override string ToString() => Id.ToString();
    }
}
