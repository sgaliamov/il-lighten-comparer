using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Shared;

namespace ILLightenComparer.Emit
{
    internal sealed class Context
    {
        private readonly ModuleBuilder _moduleBuilder;

        public Context()
        {
            var assembly = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName("ILLightenComparer.DynamicAssembly"),
                AssemblyBuilderAccess.RunAndCollect);

            _moduleBuilder = assembly.DefineDynamicModule("ILLightenComparer.Module.dll");
        }

        public CompareConfiguration Configuration { get; private set; } = new CompareConfiguration();

        public void SetConfiguration(CompareConfiguration configuration) => Configuration = configuration;

        public TypeBuilder DefineType(string name, params Type[] interfaceTypes)
        {
            var type = _moduleBuilder.DefineType(name, TypeAttributes.Sealed | TypeAttributes.NotPublic);
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

        public TReturnType CreateInstance<TReturnType>(TypeInfo typeInfo)
        {
            var factoryMethod = typeInfo
                                .GetMethod(Constants.FactoryMethodName)
                                .CreateDelegate<Func<TReturnType>>();

            return factoryMethod();
        }
    }
}
