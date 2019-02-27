using System;
using System.Collections;
using System.Collections.Generic;

namespace ILLightenComparer
{
    /// <summary>
    ///     Builds an instance of a comparer based on provided type in a method first argument.
    /// </summary>
    public interface IComparerBuilder : IComparerProvider
    {
        /// <summary>
        ///     Sugar to convert the builder to generic version.
        /// </summary>
        /// <typeparam name="T">
        ///     The type whose instances need to compare.
        ///     Defines context for the following methods.
        /// </typeparam>
        IComparerBuilder<T> For<T>();

        IComparerBuilder Configure(Action<IConfigurationBuilder> config);
    }

    /// <summary>
    ///     Builds an instance of a comparer based on provided type <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The type whose instances need to compare.</typeparam>
    public interface IComparerBuilder<T> : IComparerProvider<T>
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

        IComparerBuilder<T> Configure(Action<IConfigurationBuilder<T>> config);
    }

    public interface IConfigurationBuilder
    {
        IConfigurationBuilder SetDefaultDetectCycles(bool? value);

        IConfigurationBuilder SetDefaultIgnoreCollectionOrder(bool? value);

        IConfigurationBuilder SetDefaultIgnoredMembers(string[] value);

        IConfigurationBuilder SetDefaultIncludeFields(bool? value);

        IConfigurationBuilder SetDefaultMembersOrder(string[] value);

        IConfigurationBuilder SetDefaultStringComparisonType(StringComparison? value);

        IConfigurationBuilder SetDetectCycles(Type type, bool? value);

        IConfigurationBuilder SetIgnoreCollectionOrder(Type type, bool? value);

        IConfigurationBuilder SetIgnoredMembers(Type type, string[] value);

        IConfigurationBuilder SetIncludeFields(Type type, bool? value);

        IConfigurationBuilder SetMembersOrder(Type type, string[] value);

        IConfigurationBuilder SetStringComparisonType(Type type, StringComparison? value);

        IConfigurationBuilder SetComparer(Type type, Type comparable, IComparer comparer);

        IConfigurationBuilder<T> Configure<T>(Action<IConfigurationBuilder<T>> config);
    }

    public interface IConfigurationBuilder<T>
    {
        IConfigurationBuilder<T> SetDetectCycles(bool? value);

        IConfigurationBuilder<T> SetIgnoreCollectionOrder(bool? value);

        IConfigurationBuilder<T> SetIgnoredMembers(string[] value);

        IConfigurationBuilder<T> SetIncludeFields(bool? value);

        IConfigurationBuilder<T> SetMembersOrder(string[] value);

        IConfigurationBuilder<T> SetStringComparisonType(StringComparison? value);

        IConfigurationBuilder<T> SetComparer<TComparable>(IComparer<TComparable> comparer);
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
