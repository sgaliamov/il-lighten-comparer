using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ILLightenComparer.Config
{
    public abstract class ConfigurationBuilder : IConfigurationBuilder
    {
        private const bool DefaultIncludeFields = true;
        private const StringComparison DefaultStringComparisonType = StringComparison.Ordinal;
        private const bool DefaultDetectCycles = true;
        private const bool DefaultIgnoreCollectionOrder = false;
        private static readonly string[] DefaultIgnoredMembers = new string[0];
        private static readonly string[] DefaultMembersOrder = new string[0];

        private readonly ConcurrentDictionary<Type, Configuration> _configurations =
            new ConcurrentDictionary<Type, Configuration>();

        private readonly Configuration _default = new Configuration(
            DefaultIgnoredMembers,
            DefaultIncludeFields,
            DefaultMembersOrder,
            DefaultStringComparisonType,
            DefaultDetectCycles,
            DefaultIgnoreCollectionOrder,
            new Dictionary<Type, IComparer>(0));

        private readonly IComparerBuilder _subject;

        protected ConfigurationBuilder(IComparerBuilder subject)
        {
            _subject = subject;
        }

        public IComparerBuilder SetDefaultDetectCycles(bool? value)
        {
            _default.DetectCycles = value ?? DefaultDetectCycles;

            return _subject;
        }

        public IComparerBuilder SetDefaultIgnoreCollectionOrder(bool? value)
        {
            _default.IgnoreCollectionOrder = value ?? DefaultIgnoreCollectionOrder;

            return _subject;
        }

        public IComparerBuilder SetDefaultIgnoredMembers(string[] value)
        {
            _default.SetIgnoredMembers(value ?? DefaultIgnoredMembers);

            return _subject;
        }

        public IComparerBuilder SetDefaultIncludeFields(bool? value)
        {
            _default.IncludeFields = value ?? DefaultIncludeFields;

            return _subject;
        }

        public IComparerBuilder SetDefaultMembersOrder(string[] value)
        {
            _default.SetMembersOrder(value ?? DefaultMembersOrder);

            return _subject;
        }

        public IComparerBuilder SetDefaultStringComparisonType(StringComparison? value)
        {
            _default.StringComparisonType = value ?? DefaultStringComparisonType;

            return _subject;
        }

        public IComparerBuilder SetDetectCycles(Type type, bool? value)
        {
            var configuration = _configurations.GetOrAdd(type, _ => new Configuration(_default));
            configuration.DetectCycles = value ?? DefaultDetectCycles;

            return _subject;
        }

        public IComparerBuilder SetIgnoreCollectionOrder(Type type, bool? value)
        {
            var configuration = _configurations.GetOrAdd(type, _ => new Configuration(_default));
            configuration.IgnoreCollectionOrder = value ?? DefaultIgnoreCollectionOrder;

            return _subject;
        }

        public IComparerBuilder SetIgnoredMembers(Type type, string[] value)
        {
            var configuration = _configurations.GetOrAdd(type, _ => new Configuration(_default));
            configuration.SetIgnoredMembers(value ?? DefaultIgnoredMembers);

            return _subject;
        }

        public IComparerBuilder SetIncludeFields(Type type, bool? value)
        {
            var configuration = _configurations.GetOrAdd(type, _ => new Configuration(_default));
            configuration.IncludeFields = value ?? DefaultIncludeFields;

            return _subject;
        }

        public IComparerBuilder SetMembersOrder(Type type, string[] value)
        {
            var configuration = _configurations.GetOrAdd(type, _ => new Configuration(_default));
            configuration.SetMembersOrder(value ?? DefaultMembersOrder);

            return _subject;
        }

        public IComparerBuilder SetStringComparisonType(Type type, StringComparison? value)
        {
            var configuration = _configurations.GetOrAdd(type, _ => new Configuration(_default));
            configuration.StringComparisonType = value ?? DefaultStringComparisonType;

            return _subject;
        }

        public IComparerBuilder SetComparer(Type type, Type comparable, IComparer comparer)
        {
            var configuration = _configurations.GetOrAdd(type, _ => new Configuration(_default));
            configuration.SetComparer(comparable, comparer);

            return _subject;
        }

        internal Configuration GetConfiguration(Type type)
        {
            return _configurations.TryGetValue(type, out var configuration)
                       ? configuration
                       : _default;
        }
    }
}
