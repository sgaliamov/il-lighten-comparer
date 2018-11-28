namespace ILLightenComparer
{
    public interface IComparersBuilder : IContextBuilder
    {
        IContextBuilder SetDefaultConfiguration(CompareConfiguration configuration);
    }
}
