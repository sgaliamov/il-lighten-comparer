namespace ILLightenComparer
{
    public interface IComparersBuilder : IContextBuilder
    {
        IContextBuilder DefineDefaultConfiguration(ComparerSettings settings);
    }
}
