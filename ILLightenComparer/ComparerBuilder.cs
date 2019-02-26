using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ILLightenComparer.Emit;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer
{
    public sealed class ComparerBuilder : IComparerBuilder, IConfigurationProvider
    {
        private const bool DefaultIncludeFields = true;
        private const StringComparison DefaultStringComparisonType = StringComparison.Ordinal;
        private const bool DefaultDetectCycles = true;
        private const bool DefaultIgnoreCollectionOrder = false;
        private static readonly string[] DefaultIgnoredMembers = new string[0];
        private static readonly string[] DefaultMembersOrder = new string[0];

        private readonly ConcurrentDictionary<Type, Configuration> _configurations =
            new ConcurrentDictionary<Type, Configuration>();

        private readonly ComparerContext _context;

        private readonly Configuration _default = new Configuration(
            DefaultIgnoredMembers,
            DefaultIncludeFields,
            DefaultMembersOrder,
            DefaultStringComparisonType,
            DefaultDetectCycles,
            DefaultIgnoreCollectionOrder,
            new Dictionary<Type, IComparer>(0));

        public ComparerBuilder()
        {
            _context = new ComparerContext(this);
        }

        public IComparerBuilder SetDefaultDetectCycles(bool? value)
        {
            _default.DetectCycles = value ?? DefaultDetectCycles;

            return this;
        }

        public IComparerBuilder SetDefaultIgnoreCollectionOrder(bool? value)
        {
            _default.IgnoreCollectionOrder = value ?? DefaultIgnoreCollectionOrder;

            return this;
        }

        public IComparerBuilder SetDefaultIgnoredMembers(string[] value)
        {
            _default.SetIgnoredMembers(value ?? DefaultIgnoredMembers);

            return this;
        }

        public IComparerBuilder SetDefaultIncludeFields(bool? value)
        {
            _default.IncludeFields = value ?? DefaultIncludeFields;

            return this;
        }

        public IComparerBuilder SetDefaultMembersOrder(string[] value)
        {
            _default.SetMembersOrder(value ?? DefaultMembersOrder);

            return this;
        }

        public IComparerBuilder SetDefaultStringComparisonType(StringComparison? value)
        {
            _default.StringComparisonType = value ?? DefaultStringComparisonType;

            return this;
        }

        public IComparerBuilder SetDetectCycles(Type type, bool? value)
        {
            var configuration = _configurations.GetOrAdd(type, _ => new Configuration(_default));
            configuration.DetectCycles = value ?? DefaultDetectCycles;

            return this;
        }

        public IComparerBuilder SetIgnoreCollectionOrder(Type type, bool? value)
        {
            var configuration = _configurations.GetOrAdd(type, _ => new Configuration(_default));
            configuration.IgnoreCollectionOrder = value ?? DefaultIgnoreCollectionOrder;

            return this;
        }

        public IComparerBuilder SetIgnoredMembers(Type type, string[] value)
        {
            var configuration = _configurations.GetOrAdd(type, _ => new Configuration(_default));
            configuration.SetIgnoredMembers(value ?? DefaultIgnoredMembers);

            return this;
        }

        public IComparerBuilder SetIncludeFields(Type type, bool? value)
        {
            var configuration = _configurations.GetOrAdd(type, _ => new Configuration(_default));
            configuration.IncludeFields = value ?? DefaultIncludeFields;

            return this;
        }

        public IComparerBuilder SetMembersOrder(Type type, string[] value)
        {
            var configuration = _configurations.GetOrAdd(type, _ => new Configuration(_default));
            configuration.SetMembersOrder(value ?? DefaultMembersOrder);

            return this;
        }

        public IComparerBuilder SetStringComparisonType(Type type, StringComparison? value)
        {
            var configuration = _configurations.GetOrAdd(type, _ => new Configuration(_default));
            configuration.StringComparisonType = value ?? DefaultStringComparisonType;

            return this;
        }

        public IComparerBuilder SetComparer(Type type, Type comparable, IComparer comparer)
        {
            var configuration = _configurations.GetOrAdd(type, _ => new Configuration(_default));
            configuration.SetComparer(comparable, comparer);

            return this;
        }

        public IComparer<T> GetComparer<T>()
        {
            return (IComparer<T>)GetComparer(typeof(T));
        }

        public IComparer GetComparer(Type objectType)
        {
            var configuration = GetConfiguration(objectType);

            return _context.GetOrAdd(
                configuration,
                key => _context.GetOrBuildComparerType(key).CreateInstance<IComparerContext, IComparer>(_context));
        }

        public IEqualityComparer<T> GetEqualityComparer<T>()
        {
            throw new NotImplementedException();
        }

        public IEqualityComparer GetEqualityComparer(Type objectType)
        {
            throw new NotImplementedException();
        }

        public IComparerBuilder<T> For<T>()
        {
            return new Proxy<T>(this);
        }

        Configuration IConfigurationProvider.GetConfiguration(Type type)
        {
            return GetConfiguration(type);
        }

        private Configuration GetConfiguration(Type type)
        {
            return _configurations.TryGetValue(type, out var configuration)
                       ? configuration
                       : _default;
        }

        private sealed class Proxy<T> : IComparerBuilder<T>
        {
            private readonly ComparerBuilder _subject;

            public Proxy(ComparerBuilder subject)
            {
                _subject = subject;
            }

            public IComparerBuilder<T> SetDetectCycles(bool? value)
            {
                _subject.SetDetectCycles(typeof(T), value);
                return this;
            }

            public IComparerBuilder<T> SetIgnoreCollectionOrder(bool? value)
            {
                _subject.SetIgnoreCollectionOrder(typeof(T), value);
                return this;
            }

            public IComparerBuilder<T> SetIgnoredMembers(string[] value)
            {
                _subject.SetIgnoredMembers(typeof(T), value);
                return this;
            }

            public IComparerBuilder<T> SetIncludeFields(bool? value)
            {
                _subject.SetDefaultDetectCycles(value);
                return this;
            }

            public IComparerBuilder<T> SetMembersOrder(string[] value)
            {
                _subject.SetMembersOrder(typeof(T), value);
                return this;
            }

            public IComparerBuilder<T> SetStringComparisonType(StringComparison? value)
            {
                _subject.SetStringComparisonType(typeof(T), value);
                return this;
            }

            public IComparerBuilder<T> SetComparer<TComparable>(IComparer<TComparable> comparer)
            {
                _subject.SetComparer(typeof(T), typeof(TComparable), (IComparer)comparer);
                return this;
            }

            public IComparerBuilder<TOther> For<TOther>()
            {
                return _subject.For<TOther>();
            }

            public IComparer<T> GetComparer()
            {
                return _subject.GetComparer<T>();
            }

            public IEqualityComparer<T> GetEqualityComparer()
            {
                return _subject.GetEqualityComparer<T>();
            }
        }
    }
}
