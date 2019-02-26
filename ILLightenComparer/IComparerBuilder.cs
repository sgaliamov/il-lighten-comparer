using System;
using System.Collections;
using System.Collections.Generic;

namespace ILLightenComparer
{
    /// <summary>
    ///     Builds an instance of a comparer based on provided type in a method first argument.
    /// </summary>
    public interface IComparerBuilder : IComparerProvider, IConfigurationBuilder
    {
        /// <summary>
        ///     Sugar to convert the builder to generic version.
        /// </summary>
        /// <typeparam name="T">
        ///     The type whose instances need to compare.
        ///     Defines context for the following methods.
        /// </typeparam>
        IComparerBuilder<T> For<T>();
    }

    /// <summary>
    ///     Builds an instance of a comparer based on provided type <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The type whose instances need to compare.</typeparam>
    public interface IComparerBuilder<T> : IComparerProvider<T>, IConfigurationBuilder<T>
    {
        /// <summary>
        ///     Sugar to convert the builder to generic version.
        ///     Starts another builder context.
        /// </summary>
        /// <typeparam name="TOther">
        ///     The type whose instances need to compare.
        ///     Defines context for the following methods.
        /// </typeparam>
        IComparerBuilder<TOther> For<TOther>();
    }

    public interface IConfigurationBuilder
    {
        IComparerBuilder SetDefaultDetectCycles(bool? value);

        IComparerBuilder SetDefaultIgnoreCollectionOrder(bool? value);

        IComparerBuilder SetDefaultIgnoredMembers(string[] value);

        IComparerBuilder SetDefaultIncludeFields(bool? value);

        IComparerBuilder SetDefaultMembersOrder(string[] value);

        IComparerBuilder SetDefaultStringComparisonType(StringComparison? value);

        IComparerBuilder SetDetectCycles(Type type, bool? value);

        IComparerBuilder SetIgnoreCollectionOrder(Type type, bool? value);

        IComparerBuilder SetIgnoredMembers(Type type, string[] value);

        IComparerBuilder SetIncludeFields(Type type, bool? value);

        IComparerBuilder SetMembersOrder(Type type, string[] value);

        IComparerBuilder SetStringComparisonType(Type type, StringComparison? value);

        IComparerBuilder SetComparer(Type type, Type comparable, IComparer comparer);
    }

    public interface IConfigurationBuilder<T>
    {
        IComparerBuilder<T> SetDetectCycles(bool? value);

        IComparerBuilder<T> SetIgnoreCollectionOrder(bool? value);

        IComparerBuilder<T> SetIgnoredMembers(string[] value);

        IComparerBuilder<T> SetIncludeFields(bool? value);

        IComparerBuilder<T> SetMembersOrder(string[] value);

        IComparerBuilder<T> SetStringComparisonType(StringComparison? value);

        IComparerBuilder<T> SetComparer<TComparable>(IComparer<TComparable> comparer);
    }

    public interface IComparerProvider
    {
        IComparer GetComparer(Type objectType);

        IComparer<T> GetComparer<T>();

        IEqualityComparer GetEqualityComparer(Type objectType);

        IEqualityComparer<T> GetEqualityComparer<T>();
    }

    public interface IComparerProvider<in T>
    {
        IComparer<T> GetComparer();

        IEqualityComparer<T> GetEqualityComparer();
    }
}
