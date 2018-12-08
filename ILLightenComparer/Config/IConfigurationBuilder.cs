using System;

namespace ILLightenComparer.Config
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

    /// <summary>
    ///     Prevents multiple calls of generic version of IConfigurationBuilder&lt;in T&gt;.SetConfiguration method.
    /// </summary>
    /// <inheritdoc />
    public interface IComparerProviderOrBuilderContext<in T> : IComparerProvider<T>
    {
        /// <summary>
        ///     Sugar to convert the builder to generic version.
        ///     Starts another builder context.
        /// </summary>
        /// <typeparam name="TOther">
        ///     The type whose instances need to compare.
        ///     Defines context for the following methods.
        /// </typeparam>
        IContextBuilder<TOther> For<TOther>();
    }
}
