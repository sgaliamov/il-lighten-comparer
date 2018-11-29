using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Extensions
{
    internal static class TypeBuilderExtensions
    {
        public static MethodBuilder DefineInterfaceMethod(
            this TypeBuilder typeBuilder,
            MethodInfo interfaceMethod)
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

        public static MethodBuilder DefineStaticMethod(
            this TypeBuilder staticTypeBuilder,
            string name,
            Type returnType,
            Type[] parameterTypes) =>
            staticTypeBuilder.DefineMethod(
                name,
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.Final,
                CallingConventions.Standard,
                returnType,
                parameterTypes);

        public static TypeBuilder BuildFactoryMethod(this TypeBuilder typeBuilder)
        {
            var constructorInfo = typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

            var methodBuilder = DefineStaticMethod(typeBuilder,
                MethodName.Factory,
                typeBuilder,
                null);

            using (var il = methodBuilder.CreateILEmitter())
            {
                il.EmitCtorCall(constructorInfo);
            }

            return typeBuilder;
        }
    }
}
