namespace ILLightenComparer.Tests.EqualityTests.CycleTests.Samples
{
    public sealed class SelfSealed
    {
        public SelfSealed First;
        public SelfSealed Second { get; set; }
        public int Value { get; set; }
    }
}
