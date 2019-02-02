namespace ILLightenComparer
{
    public interface IComparerBuilder : IContextBuilder
    {
        IContextBuilder DefineDefaultConfiguration(ComparerSettings settings);
    }
}
