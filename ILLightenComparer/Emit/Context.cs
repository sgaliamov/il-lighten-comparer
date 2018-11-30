using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit
{
    public sealed class Context : IContext
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

        // todo: cache delegates
        public int LazyCompare<T>(T x, T y, HashSet<object> hash)
        {
            var compareMethod = GetStaticCompareMethod(typeof(T));

            var compare = compareMethod.CreateDelegate<Method.StaticMethodDelegate<T>>();

            return compare(this, x, y, hash);
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

        public TypeInfo GetComparerType(Type objectType)
        {
            var lazy = _comparerTypes.GetOrAdd(
                objectType,
                type => new Lazy<TypeInfo>(() => _comparerTypeBuilder.Build(type)));

            return lazy.Value;
        }

        public MethodInfo GetStaticCompareMethod(Type memberType)
        {
            var comparerType = GetComparerType(memberType);

            return comparerType.GetMethod(
                MethodName.Compare,
                Method.StaticCompareMethodParameters(memberType));
        }

        public TypeBuilder DefineType(string name, params Type[] interfaceTypes) =>
            _moduleBuilder.DefineType(name, interfaceTypes);

        internal Configuration GetConfiguration(Type type) => _configurations.TryGetValue(type, out var configuration)
            ? configuration
            : _defaultConfiguration;

        internal struct Configuration
        {
            public readonly HashSet<string> IgnoredMembers;
            public readonly bool IncludeFields;
            public readonly string[] MembersOrder;
            public readonly StringComparison StringComparisonType;

            public Configuration Mutate(ComparerSettings settings) =>
                new Configuration(
                    settings.IgnoredMembers == null
                        ? IgnoredMembers
                        : new HashSet<string>(settings.IgnoredMembers),
                    settings.IncludeFields ?? IncludeFields,
                    settings.MembersOrder ?? MembersOrder,
                    settings.StringComparisonType ?? StringComparisonType);

            public Configuration(
                HashSet<string> ignoredMembers,
                bool includeFields,
                string[] membersOrder,
                StringComparison stringComparisonType)
            {
                IgnoredMembers = ignoredMembers ?? throw new ArgumentNullException(nameof(ignoredMembers));
                IncludeFields = includeFields;
                MembersOrder = membersOrder?.Distinct().ToArray()
                               ?? throw new ArgumentNullException(nameof(membersOrder));
                StringComparisonType = stringComparisonType;
            }
        }
    }

    internal interface IContext
    {
        int LazyCompare<T>(T x, T y, HashSet<object> hash);
    }
}
