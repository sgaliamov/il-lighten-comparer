using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ILLightenComparer.Reflection;

namespace ILLightenComparer.Extensions
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

        public static TReturnType CreateInstance<T, TReturnType>(this Type type, T arg)
        {
            // todo: cache delegates?
            return type.GetMethod(MethodName.CreateInstance)
                       .CreateDelegate<Func<T, TReturnType>>()(arg);
        }

        public static TResult Create<TResult>(this Type type)
        {
            // todo: benchmark creation
            var ctor = type.GetConstructor(Type.EmptyTypes)
                       ?? throw new ArgumentException(
                           $"Type {type.DisplayName()} should has default constructor.",
                           nameof(type));

            var lambda = Expression.Lambda(typeof(Func<TResult>), Expression.New(ctor));
            var compiled = (Func<TResult>)lambda.Compile();

            return compiled();
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

        /// <summary>Returns the underlying type of the specified enumeration or Nullable.</summary>
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

        public static bool IsIntegral(this Type type)
        {
            return SmallIntegralTypes.Contains(type);
        }

        public static bool IsSealedComparable(this Type type)
        {
            return type.ImplementsGeneric(typeof(IComparable<>))
                   && (type.IsValueType || type.IsSealed);
        }

        /// <summary>
        ///     Not objects and structs.
        ///     Extended version of IsPrimitive property.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>true if the <paramref name="type">type</paramref> is one of the primitive types; otherwise, false.</returns>
        public static bool IsPrimitive(this Type type)
        {
            return type.IsPrimitive
                   || type.IsEnum
                   || ReferenceEquals(type, typeof(string))
                   || ReferenceEquals(type, typeof(decimal));
        }

        public static bool IsHierarchical(this Type type)
        {
            return !type.IsPrimitive()
                   && !type.IsSealedComparable()
                   && !type.IsNullable()
                   && !typeof(IEnumerable).IsAssignableFrom(type);
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
