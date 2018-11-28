using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ILLightenComparer
{
    public sealed class ComparersBuilder : IComparersBuilder
    {
        private readonly ConcurrentDictionary<Type, CompareConfiguration> _configurations = new ConcurrentDictionary<Type, CompareConfiguration>();
        private readonly ModuleBuilder _moduleBuilder;
        private CompareConfiguration _defaultConfiguration;

        public ComparersBuilder()
        {
            var assembly = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName("ILLightenComparer"),
                AssemblyBuilderAccess.RunAndCollect);

            _moduleBuilder = assembly.DefineDynamicModule("ILLightenComparer.dll");
        }

        public IContextBuilder SetDefaultConfiguration(CompareConfiguration configuration)
        {
            _defaultConfiguration = configuration;
            return this;
        }

        public IContextBuilder SetConfiguration(Type type, CompareConfiguration configuration)
        {
            _configurations[type] = configuration;
            return this;
        }

        public IComparer GetComparer(Type objectType) => throw new NotImplementedException();

        public IComparer<T> GetComparer<T>() => throw new NotImplementedException();

        public IEqualityComparer GetEqualityComparer(Type objectType) => throw new NotImplementedException();

        public IEqualityComparer<T> GetEqualityComparer<T>() => throw new NotImplementedException();

        public IContextBuilder<T> For<T>() => throw new NotImplementedException();
    }
}
