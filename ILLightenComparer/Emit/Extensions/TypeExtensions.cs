using System;
using System.Reflection;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Extensions
{
    internal static class TypeExtensions
    {
        public static TReturnType CreateInstance<TReturnType>(this Type type) =>
            type.GetMethod(MethodName.Factory)
                .CreateDelegate<Func<TReturnType>>()();

        public static MethodInfo GetCompareToMethod(this Type type) =>
            type.GetMethod(MethodName.CompareTo, new[] { type });

        public static Type GetUnderlyingType(this Type type)
        {
            while (true)
            {
                if (type.IsEnum)
                {
                    return Enum.GetUnderlyingType(type);
                }

                if (!type.IsNullable())
                {
                    return type;
                }

                type = type.GetGenericArguments()[0];
            }
        }

        public static bool IsNullable(this Type type) =>
            type.IsGenericType
            && !type.IsGenericTypeDefinition
            && ReferenceEquals(type.GetGenericTypeDefinition(), typeof(Nullable<>));
    }
}
