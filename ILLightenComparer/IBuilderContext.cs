using System;

namespace ILLightenComparer
{
    public interface IBuilderContext : IComparerProvider, IConfigurationSetter
    {
        IBuilderContext<T> For<T>();
        IBuilderContext For(Type type);
    }

    public interface IBuilderContext<in T> : IBuilderContext, IComparerProvider<T>, IConfigurationSetter<T>
    {
        IBuilderContext<T> For();
    }
}
