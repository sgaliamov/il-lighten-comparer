namespace ILLightenComparer.Tests.Samples
{
    public interface ISampleStruct
    {
        SampleEnum EnumProperty { get; set; }
        int KeyProperty { get; set; }
        decimal? NullableProperty { get; set; }
        string ValueProperty { get; set; }
    }
}