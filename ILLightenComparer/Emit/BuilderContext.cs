﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit
{
    internal sealed class BuilderContext<T> : BuilderContext, IBuilderContext<T>
    {
        public BuilderContext(ModuleBuilder moduleBuilder, Type objectType) : base(moduleBuilder, objectType) { }
        public BuilderContext(CompareConfiguration defaultConfiguration) : base(defaultConfiguration) { }

        public IComparer<T> GetComparer() => throw new NotImplementedException();

        public IEqualityComparer<T> GetEqualityComparer() => throw new NotImplementedException();

        public IBuilderContext<T> SetConfiguration(CompareConfiguration configuration) => throw new NotImplementedException();

        public IBuilderContext<T> For() => throw new NotImplementedException();
    }

    internal class BuilderContext : IBuilderContext
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

        public BuilderContext(CompareConfiguration defaultConfiguration) => throw new NotImplementedException();

        public CompareConfiguration Configuration { get; private set; } = new CompareConfiguration();

        public IComparer<T> GetComparer<T>() => (IComparer<T>)GetComparer(typeof(T));

        public IComparer GetComparer(Type objectType) =>
            _comparers.GetOrAdd(
                objectType,
                key => _comparerTypeBuilder.Build(key).CreateInstance<IComparer>());

        public IEqualityComparer<T> GetEqualityComparer<T>() =>
            _equalityComparerTypeBuilder.Build<T>();

        public IEqualityComparer GetEqualityComparer(Type objectType) =>
            _equalityComparerTypeBuilder.Build(objectType);

        public IBuilderContext<T> SetConfiguration<T>(CompareConfiguration configuration) => throw new NotImplementedException();

        public IBuilderContext<T> For<T>() => throw new NotImplementedException();

        public IBuilderContext For(Type type) => throw new NotImplementedException();

        public IBuilderContext SetConfiguration(CompareConfiguration configuration)
        {
            Configuration = configuration;
            return this;
        }

        public TypeBuilder DefineType(string name, params Type[] interfaceTypes) => _moduleBuilder.DefineType(name, interfaceTypes);
    }
}