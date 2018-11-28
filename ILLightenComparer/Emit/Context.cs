using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit
{
    internal sealed class Context
    {
        private readonly ComparerTypeBuilder _comparerTypeBuilder;
        private readonly Dictionary<Type, CompareConfiguration> _configurations = new Dictionary<Type, CompareConfiguration>();
        private readonly ModuleBuilder _moduleBuilder;
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

        public Type GetComparerType(Type objectType) => _comparerTypeBuilder.Build(objectType);

        public TypeBuilder DefineType(string name, params Type[] interfaceTypes) =>
            _moduleBuilder.DefineType(name, interfaceTypes);
    }
}
