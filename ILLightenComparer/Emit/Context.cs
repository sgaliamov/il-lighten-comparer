using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit
{
    internal sealed class Context
    {
        private readonly ComparerTypeBuilder _comparerTypeBuilder;
        private readonly ConcurrentDictionary<Type, CompareConfiguration> _configurations = new ConcurrentDictionary<Type, CompareConfiguration>();
        private readonly ModuleBuilder _moduleBuilder;
        private readonly ConcurrentDictionary<Type, Lazy<TypeInfo>> _comparerTypes = new ConcurrentDictionary<Type, Lazy<TypeInfo>>();
        private CompareConfiguration _defaultConfiguration = new CompareConfiguration();

        public Context(ModuleBuilder moduleBuilder)
        {
            _moduleBuilder = moduleBuilder;
            var membersProvider = new MembersProvider(this);
            _comparerTypeBuilder = new ComparerTypeBuilder(this, membersProvider);
        }

        public void SetConfiguration(Type type, CompareConfiguration configuration)
        {
            _configurations[type] = configuration;
        }

        public void SetDefaultConfiguration(CompareConfiguration configuration)
        {
            _defaultConfiguration = configuration;
        }

        public CompareConfiguration GetConfiguration(Type type) =>
            _configurations.TryGetValue(type, out var configuration)
                ? configuration
                : _defaultConfiguration;

        public Type GetComparerType(Type objectType)
        {
            var lazy = _comparerTypes.GetOrAdd(
                objectType,
                type => new Lazy<TypeInfo>(() => _comparerTypeBuilder.Build(type)));

            return lazy.Value;
        }

        public TypeBuilder DefineType(string name, params Type[] interfaceTypes) =>
            _moduleBuilder.DefineType(name, interfaceTypes);
    }
}
