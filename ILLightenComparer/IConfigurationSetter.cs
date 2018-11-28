using System;

namespace ILLightenComparer
{
    public interface IConfigurationSetter
    {
        /// <summary>
        ///     Defines the configuration for the type specified in the generic parameter of the method.
        /// </summary>
        /// <typeparam name="T">The type whose instances need to compare.</typeparam>
        /// <param name="configuration">Configuration specified for the type.</param>
        /// <returns>Typed builder context.</returns>
        IBuilderContext<T> SetConfiguration<T>(CompareConfiguration configuration);

        IBuilderContext SetConfiguration(Type type, CompareConfiguration configuration);
    }

    public interface IConfigurationSetter<in T> : IConfigurationSetter
    {
        /// <summary>
        ///     Defines the configuration for the type specified in the generic parameter of the interface.
        /// </summary>
        /// <typeparam name="T">The type whose instances need to compare.</typeparam>
        /// <param name="configuration">Configuration specified for the type.</param>
        /// <returns>Typed builder context.</returns>
        IBuilderContext<T> SetConfiguration(CompareConfiguration configuration);
    }
}
