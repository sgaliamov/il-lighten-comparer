namespace ILLightenComparer.Tests.Samples
{
    public interface ISample
    {
        SmallEnum EnumProperty { get; set; }
        int KeyProperty { get; set; }
        decimal? NullableProperty { get; set; }
        string ValueProperty { get; set; }
    }
}