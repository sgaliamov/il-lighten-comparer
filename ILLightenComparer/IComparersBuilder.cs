namespace ILLightenComparer
{
    public interface IComparersBuilder : IBuilderContext
    {
        IBuilderContext SetDefaultConfiguration(CompareConfiguration compareConfiguration);
    }
}
