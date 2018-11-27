using System;
using System.Collections.Generic;
using System.Reflection;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Extensions
{
    internal static class TypeExtensions
    {
        private static readonly HashSet<Type> SmallIntegralTypes = new HashSet<Type>(new[]
        {
            typeof(sbyte),
            typeof(byte),
            typeof(char),
            typeof(short),
            typeof(ushort)
        });

        public static TReturnType CreateInstance<TReturnType>(this Type type) =>
            type.GetMethod(MethodName.Factory)
                .CreateDelegate<Func<TReturnType>>()();

        public static MethodInfo GetCompareToMethod(this Type type)
        {
            var underlyingType = type.GetUnderlyingType();

            return underlyingType.GetMethod(MethodName.CompareTo, new[] { underlyingType });
        }

        public static Type GetUnderlyingType(this Type type)
        {
            while (true)
            {
                if (type.IsEnum)
                {
                    return Enum.GetUnderlyingType(type);
                }

                if (type.IsNullable())
                {
                    type = type.GetGenericArguments()[0];
                }
                else
                {
                    return type;
                }
            }
        }

        public static bool IsNullable(this Type type) =>
            type.IsGenericType
            && !type.IsGenericTypeDefinition
            && ReferenceEquals(type.GetGenericTypeDefinition(), typeof(Nullable<>));

        public static bool IsSmallIntegral(this Type type) =>
            !type.IsNullable() && SmallIntegralTypes.Contains(type.GetUnderlyingType());
    }
}
