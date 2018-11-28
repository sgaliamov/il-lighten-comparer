using System;

namespace ILLightenComparer
{
    public interface IBuilderContext : IBuilder
    {
        IBuilderContext SetConfiguration(CompareConfiguration configuration);
        IBuilderContext For<T>();
        IBuilderContext For(Type type);
    }
}
