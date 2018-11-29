using System;

namespace ILLightenComparer
{
    public interface IConfigurationBuilder
    {
        /// <summary>
        ///     Defines the configuration for the type specified in the argument of the method.
        ///     Builder can define multiple configurations for different types.
        /// </summary>
        IContextBuilder DefineConfiguration(Type type, ComparerSettings settings);
    }

    public interface IConfigurationBuilder<in T>
    {
        /// <summary>
        ///     Defines the configuration for the type specified in the generic parameter of the interface.
        ///     Generic version can define a configuration only for the specified type.
        /// </summary>
        IComparerProviderOrBuilderContext<T> DefineConfiguration(ComparerSettings settings);
    }
}
