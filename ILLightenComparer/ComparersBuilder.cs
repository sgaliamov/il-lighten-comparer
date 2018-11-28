using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit;

namespace ILLightenComparer
{
    public sealed class ComparersBuilder
    {
        private readonly ModuleBuilder _moduleBuilder;

        public ComparersBuilder()
        {
            var assembly = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName("ILLightenComparer.Compares"),
                AssemblyBuilderAccess.RunAndCollect);

            _moduleBuilder = assembly.DefineDynamicModule("ILLightenComparer.Compares.dll");
        }

        public IBuilderContext SetDefaultConfiguration(CompareConfiguration compareConfiguration)
        {
            return null;
        }

        public IBuilderContext For<T>() => new BuilderContext(_moduleBuilder, typeof(T));

        public IBuilderContext For(Type type) => new BuilderContext(_moduleBuilder, type);
    }
}
