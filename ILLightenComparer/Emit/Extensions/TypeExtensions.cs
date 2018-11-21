using System;
using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit.Extensions
{
    internal static class TypeExtensions
    {
        public static TReturnType CreateInstance<TReturnType>(this Type type)
        {
            var factoryMethod = type
                                .GetMethod(Constants.FactoryMethodName)
                                .CreateDelegate<Func<TReturnType>>();

            return factoryMethod();
        }
    }
}
