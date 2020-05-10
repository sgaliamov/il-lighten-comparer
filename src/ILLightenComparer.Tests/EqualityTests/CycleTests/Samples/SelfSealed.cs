using ILLightenComparer.Tests.Utilities;

namespace ILLightenComparer.Tests.EqualityTests.CycleTests.Samples
{
    public sealed class SelfSealed
    {
        public readonly int Id;
        public SelfSealed First;

        public SelfSealed Second { get; set; }
        public int Value { get; set; }

        public SelfSealed() => Id = this.GetObjectId();

        public override string ToString() => Id.ToString();
    }
}
