using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Reflection;

namespace ILLightenComparer.Emit.Types
{
    internal sealed class TypeEmitter
    {
        private readonly ModuleBuilder _module;

        public TypeEmitter()
        {
            var assembly = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName("ILLightenComparer.DynamicAssembly"),
                AssemblyBuilderAccess.Run);

            _module = assembly.DefineDynamicModule("ILLightenComparer.Module");
        }

        public Func<TReturnType> EmitFactoryMethod<TReturnType>(TypeInfo type)
        {
            var method = new DynamicMethod(
                $"InstanceOf_{type.FullName}",
                typeof(TReturnType),
                null,
                _module);

            EmitCallCtor(method.GetILGenerator(), type.GetConstructor(Type.EmptyTypes));

            return method.CreateDelegate<Func<TReturnType>>();
        }

        public MethodBuilder DefineInterfaceMethod(TypeBuilder typeBuilder, MethodInfo interfaceMethod)
        {
            var method = typeBuilder.DefineMethod(
                interfaceMethod.Name,
                MethodAttributes.Public | MethodAttributes.Virtual,
                CallingConventions.HasThis,
                interfaceMethod.ReturnType,
                interfaceMethod.GetParameters().Select(x => x.ParameterType).ToArray()
            );

            typeBuilder.DefineMethodOverride(method, interfaceMethod);

            return method;
        }

        public TypeBuilder DefineType(string name)
        {
            var type = _module.DefineType(name);
            type.AddInterfaceImplementation(Interface.Comparer);
            //type.AddInterfaceImplementation(Interface.GenericComparer);

            return type;
        }

        private static void EmitCallCtor(ILGenerator ilGenerator, ConstructorInfo constructor)
        {
            ilGenerator.Emit(OpCodes.Newobj, constructor);
            ilGenerator.Emit(OpCodes.Ret);
        }
    }
}
