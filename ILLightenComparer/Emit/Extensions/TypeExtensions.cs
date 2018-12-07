﻿using System;
using System.Collections.Generic;
using System.Linq;
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

        // todo: cache delegates
        public static TReturnType CreateInstance<T, TReturnType>(this Type type, T arg) =>
            type.GetMethod(MethodName.Factory)
                .CreateDelegate<Func<T, TReturnType>>()(arg);

        public static MethodInfo GetCompareToMethod(this Type type)
        {
            var underlyingType = type.GetUnderlyingType();

            return underlyingType.GetMethod(MethodName.CompareTo, new[] { underlyingType });
        }

        public static MethodInfo GetPropertyGetter(this Type type, string name) =>
            type.GetProperty(name)?.GetGetMethod()
            ?? throw new ArgumentException(
                $"{type.DeclaringType.DisplayName()} does not have {name} property.");

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
            type.IsValueType
            && type.IsGenericType
            && !type.IsGenericTypeDefinition
            && ReferenceEquals(type.GetGenericTypeDefinition(), typeof(Nullable<>));

        public static bool IsSmallIntegral(this Type type) => SmallIntegralTypes.Contains(type);

        public static bool IsPrimitive(this Type type) =>
            type.IsPrimitive
            || type.IsEnum
            //|| ReferenceEquals(type, typeof(IntPtr)) // todo: implement comparison for native ints
            //|| ReferenceEquals(type, typeof(UIntPtr))
            || ReferenceEquals(type, typeof(string))
            || ReferenceEquals(type, typeof(decimal));

        public static bool ImplementsGeneric(this Type type, Type generic, params Type[] typeArguments) =>
            type.GetInterfaces()
                .Any(t => t == generic.MakeGenericType(typeArguments));
    }
}
