using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ILLightenComparer
{
    /// <summary>
    ///     Interface to build an instance of a comparer based on provided type and configuration.
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
    ///     Interface to build an instance of a comparer based on provided type <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The type whose instances need to compare.</typeparam>
    public interface IComparerBuilder<T> : IComparerProvider<T>
    {
        IComparerBuilder Builder { get; }

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
    }

    public interface IConfigurationBuilder
    {
        IConfigurationBuilder DefaultDetectCycles(bool? value);

        IConfigurationBuilder DefaultIgnoreCollectionOrder(bool? value);

        IConfigurationBuilder DefaultIgnoredMembers(params string[] value);

        IConfigurationBuilder DefaultIncludeFields(bool? value);

        IConfigurationBuilder DefaultMembersOrder(params string[] value);

        IConfigurationBuilder DefaultStringComparisonType(StringComparison? value);

        IConfigurationBuilder DetectCycles(Type type, bool? value);

        IConfigurationBuilder IgnoreCollectionOrder(Type type, bool? value);

        IConfigurationBuilder IgnoreMembers(Type type, params string[] value);

        IConfigurationBuilder IncludeFields(Type type, bool? value);

        IConfigurationBuilder MembersOrder(Type type, params string[] value);

        IConfigurationBuilder StringComparisonType(Type type, StringComparison? value);

        IConfigurationBuilder SetCustomComparer<T>(IComparer<T> instance);

        IConfigurationBuilder SetCustomComparer<TComparer>();

        IConfigurationBuilder<T> ConfigureFor<T>(Action<IConfigurationBuilder<T>> config);
    }

    public interface IConfigurationBuilder<T>
    {
        IConfigurationBuilder<T> DetectCycles(bool? value);

        IConfigurationBuilder<T> IgnoreCollectionOrder(bool? value);

        IConfigurationBuilder<T> IgnoreMembers(params string[] value);

        IConfigurationBuilder<T> IgnoreMember<TMember>(Expression<Func<T, TMember>> memberSelector);

        IConfigurationBuilder<T> IncludeFields(bool? value);

        IConfigurationBuilder<T> MembersOrder(params string[] value);

        IConfigurationBuilder<T> StringComparisonType(StringComparison? value);
    }

    public interface IComparerProvider
    {
        IComparer<T> GetComparer<T>();
    }

    public interface IComparerProvider<in T>
    {
        IComparer<T> GetComparer();

        IComparer<TOther> GetComparer<TOther>();
    }
}
