using System;

namespace ILLightenComparer
{
    public interface IComparersBuilder : IComparerProvider
    {
        IBuilderContext For<T>();
        IBuilderContext For(Type type);
        IBuilderContext SetDefaultConfiguration(CompareConfiguration compareConfiguration);
    }
}
