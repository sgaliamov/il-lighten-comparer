using System;
using System.Reflection;
using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit.Extensions
{
    internal static class TypeInfoExtensions
    {
        public static TReturnType CreateInstance<TReturnType>(this TypeInfo typeInfo)
        {
            var factoryMethod = typeInfo
                                .GetMethod(Constants.FactoryMethodName)
                                .CreateDelegate<Func<TReturnType>>();

            return factoryMethod();
        }
    }
}
