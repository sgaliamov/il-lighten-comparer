using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ILLightenComparer.Reflection;
using Illuminator.Extensions;

namespace ILLightenComparer.Extensions
{
    internal static class TypeExtensions
    {
        private static readonly HashSet<Type> SmallIntegralTypes = new HashSet<Type>(new[] {
            typeof(sbyte),
            typeof(byte),
            typeof(char),
            typeof(short),
            typeof(ushort)
        });

        /// <summary>
        ///     Creates instance using static method.
        /// </summary>
        public static TReturnType CreateInstance<T, TReturnType>(this Type type, T arg) =>
            // todo: cache delegates?
            type.GetMethod(MethodName.CreateInstance)
                .CreateDelegate<Func<T, TReturnType>>()(arg);

        /// <summary>
        ///     Creates instance using default constructor.
        /// </summary>
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

        public static MethodInfo GetPropertyGetter(this Type type, string name) =>
            type.GetProperty(name)?.GetGetMethod()
            ?? throw new ArgumentException(
                $"{type.DisplayName()} does not have {name} property.");

        public static bool IsIntegral(this Type type) => SmallIntegralTypes.Contains(type);

        public static bool IsSealedComparable(this Type type) =>
            type.ImplementsGeneric(typeof(IComparable<>))
            && (type.IsValueType || type.IsSealed);

        public static bool IsHierarchical(this Type type) =>
            !type.IsPrimitive()
            && !type.IsSealedComparable()
            && !type.IsNullable()
            && !typeof(IEnumerable).IsAssignableFrom(type);
    }
}
