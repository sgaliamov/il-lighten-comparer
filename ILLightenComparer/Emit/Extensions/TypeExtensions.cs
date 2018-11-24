using System;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Extensions
{
    internal static class TypeExtensions
    {
        public static TReturnType CreateInstance<TReturnType>(this Type type)
        {
            var factoryMethod = type
                                .GetMethod(MethodName.Factory)
                                .CreateDelegate<Func<TReturnType>>();

            return factoryMethod();
        }
    }
}
