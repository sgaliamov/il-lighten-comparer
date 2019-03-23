using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ILLightenComparer.Extensions;
using ILLightenComparer.Shared;

namespace ILLightenComparer.Config
{
    using Configurations = ConcurrentDictionary<Type, Configuration>;

    internal interface IConfigurationProvider
    {
        Configuration Get(Type type);
        IComparer<T> GetCustomComparer<T>();
        bool HasCustomComparer(Type type);
    }

    internal sealed class ConfigurationProvider : IConfigurationBuilder, IConfigurationProvider
    {
        private const bool IncludeFieldsDefault = true;
        private const StringComparison StringComparisonTypeDefault = StringComparison.Ordinal;
        private const bool DetectCyclesDefault = true;
        private const bool IgnoreCollectionOrderDefault = false;
        private static readonly string[] IgnoredMembersDefault = new string[0];
        private static readonly string[] MembersOrderDefault = new string[0];

        private readonly Configurations _configurations;
        private readonly ComparersCollection _customComparers;

        private readonly Configuration _default = new Configuration(
            IgnoredMembersDefault,
            IncludeFieldsDefault,
            MembersOrderDefault,
            StringComparisonTypeDefault,
            DetectCyclesDefault,
            IgnoreCollectionOrderDefault);

        public ConfigurationProvider()
        {
            _configurations = new Configurations();
            _customComparers = new ComparersCollection();
        }

        public ConfigurationProvider(ConfigurationProvider provider)
        {
            _configurations = new Configurations(provider._configurations.ToDictionary(x => x.Key, x => new Configuration(x.Value)));
            _customComparers = new ComparersCollection(provider._customComparers);
            _default = new Configuration(provider._default);
        }

        public IConfigurationBuilder SetDefaultCyclesDetection(bool? value)
        {
            _default.DetectCycles = value ?? DetectCyclesDefault;

            return this;
        }

        public IConfigurationBuilder SetDefaultCollectionsOrderIgnoring(bool? value)
        {
            _default.IgnoreCollectionOrder = value ?? IgnoreCollectionOrderDefault;

            return this;
        }

        public IConfigurationBuilder SetDefaultIgnoredMembers(string[] value)
        {
            _default.SetIgnoredMembers(value ?? IgnoredMembersDefault);

            return this;
        }

        public IConfigurationBuilder SetDefaultFieldsInclusion(bool? value)
        {
            _default.IncludeFields = value ?? IncludeFieldsDefault;

            return this;
        }

        public IConfigurationBuilder SetDefaultMembersOrder(string[] value)
        {
            _default.SetMembersOrder(value ?? MembersOrderDefault);

            return this;
        }

        public IConfigurationBuilder SetDefaultStringComparisonType(StringComparison? value)
        {
            _default.StringComparisonType = value ?? StringComparisonTypeDefault;

            return this;
        }

        public IConfigurationBuilder DetectCycles(Type type, bool? value)
        {
            GetOrCreate(type).DetectCycles = value ?? DetectCyclesDefault;

            return this;
        }

        public IConfigurationBuilder IgnoreCollectionsOrder(Type type, bool? value)
        {
            GetOrCreate(type).IgnoreCollectionOrder = value ?? IgnoreCollectionOrderDefault;

            return this;
        }

        public IConfigurationBuilder IgnoreMembers(Type type, string[] value)
        {
            GetOrCreate(type).SetIgnoredMembers(value);

            return this;
        }

        public IConfigurationBuilder IgnoreMembers<T, TMember>(Expression<Func<T, TMember>>[] memberSelectors)
        {
            var members = memberSelectors
                          ?.Select((selector, index) =>
                          {
                              if (selector.Body.NodeType != ExpressionType.MemberAccess)
                              {
                                  throw new ArgumentException($"Member selector is expected at {index}.", nameof(memberSelectors));
                              }

                              var body = (MemberExpression)selector.Body;

                              return body.Member.Name;
                          })
                          .ToArray();


            return IgnoreMembers(typeof(T), members);
        }

        public IConfigurationBuilder IncludeFields(Type type, bool? value)
        {
            GetOrCreate(type).IncludeFields = value ?? IncludeFieldsDefault;

            return this;
        }

        public IConfigurationBuilder OrderMembers(Type type, string[] value)
        {
            GetOrCreate(type).SetMembersOrder(value ?? MembersOrderDefault);

            return this;
        }

        public IConfigurationBuilder SetStringComparisonType(Type type, StringComparison? value)
        {
            GetOrCreate(type).StringComparisonType = value ?? StringComparisonTypeDefault;

            return this;
        }

        public IConfigurationBuilder<T> ConfigureFor<T>(Action<IConfigurationBuilder<T>> config)
        {
            var proxy = new Proxy<T>(this);
            config(proxy);

            return proxy;
        }

        public IConfigurationBuilder SetCustomComparer<T>(IComparer<T> instance)
        {
            return SetCustomComparer(typeof(T), instance);
        }

        public IConfigurationBuilder SetCustomComparer<TComparer>()
        {
            var genericType = typeof(TComparer);
            var genericInterface = genericType.GetGenericInterface(typeof(IComparer<>));
            if (genericInterface == null)
            {
                throw new ArgumentException($"{nameof(TComparer)} is not generic {typeof(IComparer<>)}");
            }

            var type = genericInterface.GenericTypeArguments[0];
            var comparer = genericType.Create<TComparer>();

            return SetCustomComparer(type, comparer);
        }

        public Configuration Get(Type type)
        {
            return _configurations.TryGetValue(type, out var configuration)
                       ? configuration
                       : _default;
        }

        public IComparer<T> GetCustomComparer<T>()
        {
            return _customComparers.TryGetValue(typeof(T), out var comparer) ? (IComparer<T>)comparer : null;
        }

        public bool HasCustomComparer(Type type)
        {
            return _customComparers.ContainsKey(type);
        }

        private ConfigurationProvider SetCustomComparer(Type type, object instance)
        {
            if (instance == null)
            {
                _customComparers.TryRemove(type, out _);
            }
            else
            {
                _customComparers.AddOrUpdate(type, key => instance, (key, _) => instance);
            }

            return this;
        }

        private Configuration GetOrCreate(Type type)
        {
            return _configurations.GetOrAdd(type, _ => new Configuration(_default));
        }

        private sealed class Proxy<T> : IConfigurationBuilder<T>
        {
            private readonly ConfigurationProvider _subject;

            public Proxy(ConfigurationProvider subject)
            {
                _subject = subject;
            }

            public IConfigurationBuilder<T> DetectCycles(bool? value)
            {
                _subject.DetectCycles(typeof(T), value);

                return this;
            }

            public IConfigurationBuilder<T> IgnoreCollectionsOrder(bool? value)
            {
                _subject.IgnoreCollectionsOrder(typeof(T), value);

                return this;
            }

            public IConfigurationBuilder<T> IgnoreMembers(string[] value)
            {
                _subject.IgnoreMembers(typeof(T), value);

                return this;
            }

            public IConfigurationBuilder<T> IgnoreMembers<TMember>(Expression<Func<T, TMember>>[] memberSelectors)
            {
                _subject.IgnoreMembers(memberSelectors);

                return this;
            }

            public IConfigurationBuilder<T> IncludeFields(bool? value)
            {
                _subject.IncludeFields(typeof(T), value);

                return this;
            }

            public IConfigurationBuilder<T> OrderMembers(string[] value)
            {
                _subject.OrderMembers(typeof(T), value);

                return this;
            }

            public IConfigurationBuilder<T> SetStringComparisonType(StringComparison? value)
            {
                _subject.SetStringComparisonType(typeof(T), value);

                return this;
            }
        }
    }
}
