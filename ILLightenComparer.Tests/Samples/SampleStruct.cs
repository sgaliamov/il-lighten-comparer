namespace ILLightenComparer.Tests.Samples
{
    public struct SampleStruct<TMember>
    {
        public TMember Field;

        public TMember Property { get; set; }

        public override string ToString()
        {
            return $"{{ {Field}, {Property} }}";
        }
    }
}
