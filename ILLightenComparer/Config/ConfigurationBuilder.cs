using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using ILLightenComparer.Extensions;

namespace ILLightenComparer.Config
{
    using Configurations = ConcurrentDictionary<Type, Configuration>;

    internal interface IConfigurationProvider
    {
        Configuration Get(Type type);
    }

    internal sealed class ConfigurationBuilder : IConfigurationBuilder, IConfigurationProvider
    {
        private const bool IncludeFieldsDefault = true;
        private const StringComparison StringComparisonTypeDefault = StringComparison.Ordinal;
        private const bool DetectCyclesDefault = true;
        private const bool IgnoreCollectionOrderDefault = false;
        private static readonly string[] IgnoredMembersDefault = new string[0];
        private static readonly string[] MembersOrderDefault = new string[0];
        private readonly Configurations _configurations = new Configurations();

        private readonly Configuration _default = new Configuration(
            IgnoredMembersDefault,
            IncludeFieldsDefault,
            MembersOrderDefault,
            StringComparisonTypeDefault,
            DetectCyclesDefault,
            IgnoreCollectionOrderDefault,
            new Dictionary<Type, object>(0));

        public IConfigurationBuilder DefaultDetectCycles(bool? value)
        {
            _default.DetectCycles = value ?? DetectCyclesDefault;

            return this;
        }

        public IConfigurationBuilder DefaultIgnoreCollectionOrder(bool? value)
        {
            _default.IgnoreCollectionOrder = value ?? IgnoreCollectionOrderDefault;

            return this;
        }

        public IConfigurationBuilder DefaultIgnoredMembers(string[] value)
        {
            _default.SetIgnoredMembers(value ?? IgnoredMembersDefault);

            return this;
        }

        public IConfigurationBuilder DefaultIncludeFields(bool? value)
        {
            _default.IncludeFields = value ?? IncludeFieldsDefault;

            return this;
        }

        public IConfigurationBuilder DefaultMembersOrder(string[] value)
        {
            _default.SetMembersOrder(value ?? MembersOrderDefault);

            return this;
        }

        public IConfigurationBuilder DefaultStringComparisonType(StringComparison? value)
        {
            _default.StringComparisonType = value ?? StringComparisonTypeDefault;

            return this;
        }

        public IConfigurationBuilder DetectCycles(Type type, bool? value)
        {
            GetOrCreate(type).DetectCycles = value ?? DetectCyclesDefault;

            return this;
        }

        public IConfigurationBuilder IgnoreCollectionOrder(Type type, bool? value)
        {
            GetOrCreate(type).IgnoreCollectionOrder = value ?? IgnoreCollectionOrderDefault;

            return this;
        }

        public IConfigurationBuilder IgnoredMembers(Type type, string[] value)
        {
            GetOrCreate(type).SetIgnoredMembers(value ?? IgnoredMembersDefault);

            return this;
        }

        public IConfigurationBuilder IncludeFields(Type type, bool? value)
        {
            GetOrCreate(type).IncludeFields = value ?? IncludeFieldsDefault;

            return this;
        }

        public IConfigurationBuilder MembersOrder(Type type, string[] value)
        {
            GetOrCreate(type).SetMembersOrder(value ?? MembersOrderDefault);

            return this;
        }

        public IConfigurationBuilder StringComparisonType(Type type, StringComparison? value)
        {
            GetOrCreate(type).StringComparisonType = value ?? StringComparisonTypeDefault;

            return this;
        }

        public IConfigurationBuilder SetComparer<TComparer>(Type type, TComparer instance)
        {
            var configuration = GetOrCreate(type);

            // todo: optimize
            typeof(Configuration)
                .GetMethod(nameof(Configuration.SetComparer))
                ?.MakeGenericMethod(type)
                .Invoke(configuration, new object[] { instance });

            return this;
        }

        public IConfigurationBuilder SetComparer<TComparer>(Type type)
        {
            // todo: cache
            var comparerType = typeof(TComparer);
            var ctor = comparerType.GetConstructor(Type.EmptyTypes)
                       ?? throw new ArgumentException(
                           $"Comparer {comparerType.DisplayName()} should have default constructor.",
                           nameof(type));
            var lambda = Expression.Lambda(typeof(Func<TComparer>), Expression.New(ctor));
            var compiled = (Func<TComparer>)lambda.Compile();

            return SetComparer(type, compiled());
        }

        public IConfigurationBuilder<T> Configure<T>(Action<IConfigurationBuilder<T>> config)
        {
            var proxy = new Proxy<T>(this);
            config(proxy);

            return proxy;
        }

        public Configuration Get(Type type)
        {
            return _configurations.TryGetValue(type, out var configuration)
                       ? configuration
                       : _default;
        }

        private Configuration GetOrCreate(Type type)
        {
            return _configurations.GetOrAdd(type, _ => new Configuration(_default));
        }

        private class Proxy<T> : IConfigurationBuilder<T>
        {
            private readonly ConfigurationBuilder _subject;

            public Proxy(ConfigurationBuilder subject)
            {
                _subject = subject;
            }

            public IConfigurationBuilder<T> DetectCycles(bool? value)
            {
                _subject.DetectCycles(typeof(T), value);

                return this;
            }

            public IConfigurationBuilder<T> IgnoreCollectionOrder(bool? value)
            {
                _subject.IgnoreCollectionOrder(typeof(T), value);

                return this;
            }

            public IConfigurationBuilder<T> IgnoredMembers(string[] value)
            {
                _subject.IgnoredMembers(typeof(T), value);

                return this;
            }

            public IConfigurationBuilder<T> IncludeFields(bool? value)
            {
                _subject.IncludeFields(typeof(T), value);

                return this;
            }

            public IConfigurationBuilder<T> MembersOrder(string[] value)
            {
                _subject.MembersOrder(typeof(T), value);

                return this;
            }

            public IConfigurationBuilder<T> StringComparisonType(StringComparison? value)
            {
                _subject.StringComparisonType(typeof(T), value);

                return this;
            }

            public IConfigurationBuilder<T> SetComparer(IComparer<T> instance)
            {
                _subject.SetComparer(typeof(T), instance);

                return this;
            }

            public IConfigurationBuilder<T> SetComparer<TComparer>() where TComparer : IComparer<T>
            {
                _subject.SetComparer<TComparer>(typeof(T));

                return this;
            }
        }
    }
}
