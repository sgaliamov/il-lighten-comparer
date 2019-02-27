using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ILLightenComparer.Config
{
    using Configurations = ConcurrentDictionary<Type, Configuration>;

    internal interface IConfigurationProvider
    {
        Configuration Get(Type type);
    }

    internal sealed class ConfigurationBuilder : IConfigurationBuilder, IConfigurationProvider
    {
        private const bool DefaultIncludeFields = true;
        private const StringComparison DefaultStringComparisonType = StringComparison.Ordinal;
        private const bool DefaultDetectCycles = true;
        private const bool DefaultIgnoreCollectionOrder = false;
        private static readonly string[] DefaultIgnoredMembers = new string[0];
        private static readonly string[] DefaultMembersOrder = new string[0];
        private readonly Configurations _configurations = new Configurations();

        private readonly Configuration _default = new Configuration(
            DefaultIgnoredMembers,
            DefaultIncludeFields,
            DefaultMembersOrder,
            DefaultStringComparisonType,
            DefaultDetectCycles,
            DefaultIgnoreCollectionOrder,
            new Dictionary<Type, object>(0));

        public IConfigurationBuilder SetDefaultDetectCycles(bool? value)
        {
            _default.DetectCycles = value ?? DefaultDetectCycles;

            return this;
        }

        public IConfigurationBuilder SetDefaultIgnoreCollectionOrder(bool? value)
        {
            _default.IgnoreCollectionOrder = value ?? DefaultIgnoreCollectionOrder;

            return this;
        }

        public IConfigurationBuilder SetDefaultIgnoredMembers(string[] value)
        {
            _default.SetIgnoredMembers(value ?? DefaultIgnoredMembers);

            return this;
        }

        public IConfigurationBuilder SetDefaultIncludeFields(bool? value)
        {
            _default.IncludeFields = value ?? DefaultIncludeFields;

            return this;
        }

        public IConfigurationBuilder SetDefaultMembersOrder(string[] value)
        {
            _default.SetMembersOrder(value ?? DefaultMembersOrder);

            return this;
        }

        public IConfigurationBuilder SetDefaultStringComparisonType(StringComparison? value)
        {
            _default.StringComparisonType = value ?? DefaultStringComparisonType;

            return this;
        }

        public IConfigurationBuilder SetDetectCycles(Type type, bool? value)
        {
            GetOrCreate(type).DetectCycles = value ?? DefaultDetectCycles;

            return this;
        }

        public IConfigurationBuilder SetIgnoreCollectionOrder(Type type, bool? value)
        {
            GetOrCreate(type).IgnoreCollectionOrder = value ?? DefaultIgnoreCollectionOrder;

            return this;
        }

        public IConfigurationBuilder SetIgnoredMembers(Type type, string[] value)
        {
            GetOrCreate(type).SetIgnoredMembers(value ?? DefaultIgnoredMembers);

            return this;
        }

        public IConfigurationBuilder SetIncludeFields(Type type, bool? value)
        {
            GetOrCreate(type).IncludeFields = value ?? DefaultIncludeFields;

            return this;
        }

        public IConfigurationBuilder SetMembersOrder(Type type, string[] value)
        {
            GetOrCreate(type).SetMembersOrder(value ?? DefaultMembersOrder);

            return this;
        }

        public IConfigurationBuilder SetStringComparisonType(Type type, StringComparison? value)
        {
            GetOrCreate(type).StringComparisonType = value ?? DefaultStringComparisonType;

            return this;
        }

        public IConfigurationBuilder SetComparer<TComparable>(Type type, IComparer<TComparable> comparer)
        {
            GetOrCreate(type).SetComparer(comparer);

            return this;
        }

        public IConfigurationBuilder<T> Configure<T>(Action<IConfigurationBuilder<T>> config)
        {
            return new Proxy<T>(this);
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

            public IConfigurationBuilder<T> SetDetectCycles(bool? value)
            {
                _subject.SetDetectCycles(typeof(T), value);

                return this;
            }

            public IConfigurationBuilder<T> SetIgnoreCollectionOrder(bool? value)
            {
                _subject.SetIgnoreCollectionOrder(typeof(T), value);

                return this;
            }

            public IConfigurationBuilder<T> SetIgnoredMembers(string[] value)
            {
                _subject.SetIgnoredMembers(typeof(T), value);

                return this;
            }

            public IConfigurationBuilder<T> SetIncludeFields(bool? value)
            {
                _subject.SetIncludeFields(typeof(T), value);

                return this;
            }

            public IConfigurationBuilder<T> SetMembersOrder(string[] value)
            {
                _subject.SetMembersOrder(typeof(T), value);

                return this;
            }

            public IConfigurationBuilder<T> SetStringComparisonType(StringComparison? value)
            {
                _subject.SetStringComparisonType(typeof(T), value);

                return this;
            }

            public IConfigurationBuilder<T> SetComparer<TComparable>(IComparer<TComparable> comparer)
            {
                _subject.SetComparer(typeof(T), comparer);

                return this;
            }
        }
    }
}
