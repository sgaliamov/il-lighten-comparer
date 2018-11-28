using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit
{
    internal class BuilderContext
    {
        private readonly ConcurrentDictionary<Type, IComparer> _comparers = new ConcurrentDictionary<Type, IComparer>();
        private readonly ComparerTypeBuilder _comparerTypeBuilder;
        private readonly EqualityComparerTypeBuilder _equalityComparerTypeBuilder;
        private readonly ModuleBuilder _moduleBuilder;
        private readonly Type _objectType;

        public BuilderContext(ModuleBuilder moduleBuilder, Type objectType)
        {
            _moduleBuilder = moduleBuilder;
            _objectType = objectType;
            var membersProvider = new MembersProvider(this);
            _equalityComparerTypeBuilder = new EqualityComparerTypeBuilder(this, membersProvider);
            _comparerTypeBuilder = new ComparerTypeBuilder(this, membersProvider);
        }

        public CompareConfiguration Configuration { get; } = new CompareConfiguration();

        public IComparer<T> GetComparer<T>() => (IComparer<T>)GetComparer(typeof(T));

        public IComparer GetComparer(Type objectType) =>
            _comparers.GetOrAdd(
                objectType,
                key => _comparerTypeBuilder.Build(key).CreateInstance<IComparer>());

        public IEqualityComparer<T> GetEqualityComparer<T>() =>
            _equalityComparerTypeBuilder.Build<T>();

        public IEqualityComparer GetEqualityComparer(Type objectType) =>
            _equalityComparerTypeBuilder.Build(objectType);

        public IContextBuilder<T> For<T>() => throw new NotImplementedException();

        public TypeBuilder DefineType(string name, params Type[] interfaceTypes) => _moduleBuilder.DefineType(name, interfaceTypes);
    }
}
