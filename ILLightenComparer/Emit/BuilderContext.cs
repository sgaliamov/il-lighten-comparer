using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit
{
    internal sealed class BuilderContext : IBuilderContext
    {
        private readonly ModuleBuilder _moduleBuilder;
        private readonly ConcurrentDictionary<Type, IComparer> _comparers = new ConcurrentDictionary<Type, IComparer>();
        private readonly ComparerTypeBuilder _comparerTypeBuilder;
        private readonly EqualityComparerTypeBuilder _equalityComparerTypeBuilder;

        public BuilderContext(ModuleBuilder moduleBuilder, Type type)
        {
            _moduleBuilder = moduleBuilder;
            var membersProvider = new MembersProvider(this);
            _equalityComparerTypeBuilder = new EqualityComparerTypeBuilder(this, membersProvider);
            _comparerTypeBuilder = new ComparerTypeBuilder(this, membersProvider);
        }

        public CompareConfiguration Configuration { get; private set; } = new CompareConfiguration();

        public IBuilderContext SetConfiguration(CompareConfiguration configuration)
        {
            Configuration = configuration;
            return this;
        }

        public TypeBuilder DefineType(string name, params Type[] interfaceTypes)
        {
            var type = _moduleBuilder.DefineType(name, TypeAttributes.Sealed | TypeAttributes.Public);
            if (interfaceTypes == null)
            {
                return type;
            }

            foreach (var interfaceType in interfaceTypes)
            {
                type.AddInterfaceImplementation(interfaceType);
            }

            return type;
        }
        
        public IComparer<T> GetComparer<T>() => (IComparer<T>)GetComparer(typeof(T));

        public IComparer GetComparer(Type objectType) =>
            _comparers.GetOrAdd(
                objectType,
                key => _comparerTypeBuilder.Build(key).CreateInstance<IComparer>());

        public IEqualityComparer<T> GetEqualityComparer<T>() =>
            _equalityComparerTypeBuilder.Build<T>();

        public IEqualityComparer GetEqualityComparer(Type objectType) =>
            _equalityComparerTypeBuilder.Build(objectType);
    }
}
