using ILLightenComparer.Tests.Utilities;

namespace ILLightenComparer.Tests.Samples
{
    public sealed class SampleObject<TMember>
    {
        public TMember Field;
        public TMember Property { get; set; }

        public override string ToString()
        {
            var field = Field.ToStringEx();
            var property = Property.ToStringEx();

            return $"{{ Field: {field}, Property: {property} }}";
        }
    }
}
