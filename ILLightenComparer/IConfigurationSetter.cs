namespace ILLightenComparer
{
    public interface IConfigurationSetter<in T>
    {
        IBuilderContext<T> SetConfiguration(CompareConfiguration configuration);
    }

    public interface IConfigurationSetter
    {
        IBuilderContext<T> SetConfiguration<T>(CompareConfiguration configuration);
    }
}
