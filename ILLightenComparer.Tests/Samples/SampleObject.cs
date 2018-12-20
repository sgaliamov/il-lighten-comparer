namespace ILLightenComparer.Tests.Samples
{
    public sealed class SampleObject<TMember>
    {
        public TMember Field;

        public TMember Property { get; set; }

        public override string ToString()
        {
            return $"{{ {Field}, {Property} }}";
        }
    }
}
