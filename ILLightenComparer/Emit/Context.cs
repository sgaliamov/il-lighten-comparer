using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit
{
    internal sealed class Context
    {
        private readonly ComparerTypeBuilder _comparerTypeBuilder;

        private readonly ConcurrentDictionary<Type, Lazy<TypeInfo>> _comparerTypes =
            new ConcurrentDictionary<Type, Lazy<TypeInfo>>();

        private readonly ConcurrentDictionary<Type, Configuration> _configurations =
            new ConcurrentDictionary<Type, Configuration>();

        private readonly ModuleBuilder _moduleBuilder;

        private Configuration _defaultConfiguration = new Configuration(
            new HashSet<string>(),
            false,
            new string[0],
            StringComparison.Ordinal);

        public Context(ModuleBuilder moduleBuilder)
        {
            _moduleBuilder = moduleBuilder;
            var membersProvider = new MembersProvider(this);
            _comparerTypeBuilder = new ComparerTypeBuilder(this, membersProvider);
        }

        public void DefineConfiguration(Type type, ComparerSettings settings)
        {
            _configurations.AddOrUpdate(
                type,
                _ => _defaultConfiguration.Mutate(settings),
                (_, configuration) => configuration.Mutate(settings));
        }

        public void DefineDefaultConfiguration(ComparerSettings settings)
        {
            _defaultConfiguration = _defaultConfiguration.Mutate(settings);
        }

        public Configuration GetConfiguration(Type type) => _configurations.TryGetValue(type, out var configuration)
            ? configuration
            : _defaultConfiguration;

        public TypeInfo GetComparerType(Type objectType)
        {
            var lazy = _comparerTypes.GetOrAdd(
                objectType,
                type => new Lazy<TypeInfo>(() => _comparerTypeBuilder.Build(type)));

            return lazy.Value;
        }

        public TypeBuilder DefineType(string name, params Type[] interfaceTypes) =>
            _moduleBuilder.DefineType(name, interfaceTypes);

        internal struct Configuration
        {
            public readonly HashSet<string> IgnoredMembers;
            public readonly bool IncludeFields;
            public readonly string[] MembersOrder;
            public readonly StringComparison StringComparisonType;

            public Configuration Mutate(ComparerSettings settings) =>
                new Configuration(
                    settings.IgnoredMembers ?? IgnoredMembers,
                    settings.IncludeFields ?? IncludeFields,
                    settings.MembersOrder ?? MembersOrder,
                    settings.StringComparisonType ?? StringComparisonType);

            public Configuration(
                HashSet<string> ignoredMembers,
                bool includeFields,
                string[] membersOrder,
                StringComparison stringComparisonType)
            {
                IgnoredMembers = ignoredMembers;
                IncludeFields = includeFields;
                MembersOrder = membersOrder;
                StringComparisonType = stringComparisonType;
            }
        }
    }
}
