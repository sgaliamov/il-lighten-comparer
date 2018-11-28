using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit;

namespace ILLightenComparer
{
    public sealed class ComparersBuilder : IComparersBuilder
    {
        private readonly ModuleBuilder _moduleBuilder;

        public ComparersBuilder()
        {
            var assembly = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName("ILLightenComparer.Compares"),
                AssemblyBuilderAccess.RunAndCollect);

            _moduleBuilder = assembly.DefineDynamicModule("ILLightenComparer.Compares.dll");
        }

        public IBuilderContext SetDefaultConfiguration(CompareConfiguration configuration) =>
            new BuilderContext(configuration);

        public IBuilderContext For<T>() => new BuilderContext(_moduleBuilder, typeof(T));

        public IBuilderContext For(Type type) => new BuilderContext(_moduleBuilder, type);

        public IComparer GetComparer(Type objectType) => throw new NotImplementedException();

        public IComparer<T> GetComparer<T>() => throw new NotImplementedException();

        public IEqualityComparer GetEqualityComparer(Type objectType) => throw new NotImplementedException();

        public IEqualityComparer<T> GetEqualityComparer<T>() => throw new NotImplementedException();
    }
}
