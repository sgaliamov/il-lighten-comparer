using System;
using System.Collections;
using System.Collections.Generic;

namespace ILLightenComparer
{
    /// <summary>
    ///     Builds an instance of a comparer based on provided type in a method first argument.
    /// </summary>
    public interface IComparerBuilder
    {
        IComparerBuilder DefineDefaultConfiguration(ComparerSettings settings);

        /// <summary>
        ///     Defines the configuration for the type specified in the argument of the method.
        ///     Builder can define multiple configurations for different types.
        /// </summary>
        IComparerBuilder DefineConfiguration(Type type, ComparerSettings settings);

        IComparerBuilder SetComparer(Type type, IComparer comparer);

        /// <summary>
        ///     Sugar to convert the builder to generic version.
        /// </summary>
        /// <typeparam name="T">
        ///     The type whose instances need to compare.
        ///     Defines context for the following methods.
        /// </typeparam>
        IComparerBuilder<T> For<T>();

        IComparer GetComparer(Type objectType);

        IComparer<T> GetComparer<T>();

        IEqualityComparer GetEqualityComparer(Type objectType);

        IEqualityComparer<T> GetEqualityComparer<T>();
    }

    /// <summary>
    ///     Builds an instance of a comparer based on provided type <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The type whose instances need to compare.</typeparam>
    public interface IComparerBuilder<T>
    {
        /// <summary>
        ///     Defines the configuration for the type specified in the generic parameter of the interface.
        ///     Generic version can define a configuration only for the specified type.
        /// </summary>
        IComparerBuilder<T> DefineConfiguration(ComparerSettings settings);

        IComparerBuilder<T> SetComparer(IComparer<T> comparer);

        /// <summary>
        ///     Sugar to convert the builder to generic version.
        ///     Starts another builder context.
        /// </summary>
        /// <typeparam name="TOther">
        ///     The type whose instances need to compare.
        ///     Defines context for the following methods.
        /// </typeparam>
        IComparerBuilder<TOther> For<TOther>();

        IComparer<T> GetComparer();

        IEqualityComparer<T> GetEqualityComparer();
    }
}
