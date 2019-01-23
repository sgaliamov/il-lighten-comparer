﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Extensions
{
    internal static class TypeExtensions
    {
        // todo: if https://stackoverflow.com/questions/23833999/why-int-maxvalue-int-minvalue-1 then include int
        private static readonly HashSet<Type> SmallIntegralTypes = new HashSet<Type>(new[]
        {
            typeof(sbyte),
            typeof(byte),
            typeof(char),
            typeof(short),
            typeof(ushort)
        });

        // todo: cache delegates
        public static TReturnType CreateInstance<T, TReturnType>(this Type type, T arg)
        {
            return type.GetMethod(MethodName.CreateInstance)
                       .CreateDelegate<Func<T, TReturnType>>()(arg);
        }

        public static MethodInfo GetUnderlyingCompareToMethod(this Type type)
        {
            var underlyingType = type.GetUnderlyingType();

            return underlyingType.GetMethod(MethodName.CompareTo, new[] { underlyingType });
        }

        public static MethodInfo GetPropertyGetter(this Type type, string name)
        {
            return type
                   .GetProperty(name)
                   ?.GetGetMethod()
                   ?? throw new ArgumentException(
                       $"{type.DisplayName()} does not have {name} property.");
        }

        /// <summary>
        ///     <summary>Returns the underlying type of the specified enumeration or Nullable.</summary>
        /// </summary>
        /// <param name="type">The type whose underlying type will be retrieved.</param>
        /// <returns>The underlying type of <paramref name="type">type</paramref>.</returns>
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

        public static bool IsNullable(this Type type)
        {
            return type.IsValueType
                   && type.IsGenericType
                   && !type.IsGenericTypeDefinition
                   && ReferenceEquals(type.GetGenericTypeDefinition(), typeof(Nullable<>));
        }

        public static bool IsSmallIntegral(this Type type)
        {
            return SmallIntegralTypes.Contains(type);
        }

        public static bool IsSealedComparable(this Type type)
        {
            return type.ImplementsGeneric(typeof(IComparable<>))
                   && (type.IsValueType || type.IsSealed);
        }

        public static bool IsCollectionOfSealed(this Type objectType)
        {
            var generic = objectType.GetGenericInterface(typeof(IEnumerable<>));
            if (generic == null)
            {
                return false;
            }

            var itemType = generic.GetGenericArguments()[0].GetUnderlyingType();

            return itemType.IsPrimitive() || itemType.IsSealedComparable();
        }

        public static bool IsPrimitive(this Type type)
        {
            return type.IsPrimitive
                   || type.IsEnum
                   //|| ReferenceEquals(type, typeof(IntPtr)) // todo: implement comparison for native ints
                   //|| ReferenceEquals(type, typeof(UIntPtr))
                   || ReferenceEquals(type, typeof(string))
                   || ReferenceEquals(type, typeof(decimal));
        }

        public static bool ImplementsGeneric(this Type type, Type generic)
        {
            return type.GetGenericInterface(generic) != null;
        }

        public static Type GetGenericInterface(this Type type, Type generic)
        {
            if (!generic.IsGenericType)
            {
                throw new ArgumentException($"{generic.DisplayName()} should be generic type.", nameof(generic));
            }

            if (type.IsInterface && type.IsGenericType && type.GetGenericTypeDefinition() == generic)
            {
                return type;
            }

            return type.GetInterfaces().FirstOrDefault(t => t.IsGenericType && generic == t.GetGenericTypeDefinition());
        }
    }
}
