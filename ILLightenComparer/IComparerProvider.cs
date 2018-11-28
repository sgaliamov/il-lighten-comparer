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
    /// <inheritdoc />
    public interface IComparerProvider<in T> : IComparerProvider
    {
        IComparer<T> GetComparer();
        IEqualityComparer<T> GetEqualityComparer();
    }
}
