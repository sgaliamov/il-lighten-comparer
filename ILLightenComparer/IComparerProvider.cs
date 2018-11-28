using System;
using System.Collections;
using System.Collections.Generic;

namespace ILLightenComparer
{
    /// <summary>
    ///     Provides an instance of a comparer based on provided type in a method argument.
    /// </summary>
    public interface IComparerProvider
    {
        IComparer GetComparer(Type objectType);
        IComparer<T> GetComparer<T>();
        IEqualityComparer GetEqualityComparer(Type objectType);
        IEqualityComparer<T> GetEqualityComparer<T>();
    }

    /// <summary>
    ///     Provides an instance of a comparer based on provided type in generic parameter.
    /// </summary>
    /// <typeparam name="T">The type whose instances need to compare.</typeparam>
    public interface IComparerProvider<in T>
    {
        IComparer<T> GetComparer();
        IEqualityComparer<T> GetEqualityComparer();
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
