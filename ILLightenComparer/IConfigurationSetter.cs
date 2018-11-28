using System;

namespace ILLightenComparer
{
    public interface IConfigurationSetter
    {
        /// <summary>
        ///     Defines the configuration for the type specified in the argument of the method.
        /// </summary>
        IBuilderContext SetConfiguration(Type type, CompareConfiguration configuration);
    }

    public interface IConfigurationSetter<in T>
    {
        /// <summary>
        ///     Defines the configuration for the type specified in the generic parameter of the interface.
        /// </summary>
        IBuilderContext<T> SetConfiguration(CompareConfiguration configuration);
    }
}
