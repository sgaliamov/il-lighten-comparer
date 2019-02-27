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

        IComparerBuilder<T> For<T>(Action<IConfigurationBuilder<T>> config);

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

        IComparerBuilder<TOther> For<TOther>(Action<IConfigurationBuilder<TOther>> config);

        IComparerBuilder<T> Configure(Action<IConfigurationBuilder<T>> config);

        IComparerBuilder Builder { get; }
    }

    public interface IConfigurationBuilder
    {
        IConfigurationBuilder DefaultDetectCycles(bool? value);

        IConfigurationBuilder DefaultIgnoreCollectionOrder(bool? value);

        IConfigurationBuilder DefaultIgnoredMembers(string[] value);

        IConfigurationBuilder DefaultIncludeFields(bool? value);

        IConfigurationBuilder DefaultMembersOrder(string[] value);

        IConfigurationBuilder DefaultStringComparisonType(StringComparison? value);

        IConfigurationBuilder DetectCycles(Type type, bool? value);

        IConfigurationBuilder IgnoreCollectionOrder(Type type, bool? value);

        IConfigurationBuilder IgnoredMembers(Type type, string[] value);

        IConfigurationBuilder IncludeFields(Type type, bool? value);

        IConfigurationBuilder MembersOrder(Type type, string[] value);

        IConfigurationBuilder StringComparisonType(Type type, StringComparison? value);

        IConfigurationBuilder Comparer<TComparable>(Type type, IComparer<TComparable> comparer);

        IConfigurationBuilder<T> Configure<T>(Action<IConfigurationBuilder<T>> config);
    }

    public interface IConfigurationBuilder<T>
    {
        IConfigurationBuilder<T> DetectCycles(bool? value);

        IConfigurationBuilder<T> IgnoreCollectionOrder(bool? value);

        IConfigurationBuilder<T> IgnoredMembers(string[] value);

        IConfigurationBuilder<T> IncludeFields(bool? value);

        IConfigurationBuilder<T> MembersOrder(string[] value);

        IConfigurationBuilder<T> StringComparisonType(StringComparison? value);

        IConfigurationBuilder<T> Comparer<TComparable>(IComparer<TComparable> comparer);
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
