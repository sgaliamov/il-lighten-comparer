using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

        private static readonly HashSet<Type> CeqCompatibleTypes = new HashSet<Type>(new[] {
            typeof(sbyte),
            typeof(byte),
            typeof(char),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double)
        });

        private static readonly HashSet<Type> BasicTypes = new HashSet<Type>(typeof(object).Assembly
            .GetTypes()
            .Where(x => x.FullName.StartsWith("System.")));

        /// <summary>
        ///     Creates instance using static method.
        /// </summary>
        public static TReturnType CreateInstance<T, TReturnType>(this Type type, T arg) => type
            .GetMethod(nameof(CreateInstance))
            .CreateDelegate<Func<T, TReturnType>>()(arg); // todo: 1. cache delegates?

        /// <summary>
        ///     Creates instance using default constructor.
        /// </summary>
        public static TResult Create<TResult>(this Type type)
        {
            // todo: 2. benchmark creation
            var ctor = type.GetConstructor(Type.EmptyTypes)
                ?? throw new ArgumentException($"Type {type.DisplayName()} should has default constructor.", nameof(type));

            var lambda = Expression.Lambda(typeof(Func<TResult>), Expression.New(ctor));
            var compiled = (Func<TResult>)lambda.Compile();

            return compiled();
        }

        public static MethodInfo GetUnderlyingCompareToMethod(this Type type)
        {
            var underlyingType = type.GetUnderlyingType();

            return underlyingType.FindMethod(nameof(IComparable.CompareTo), new[] { underlyingType });
        }

        public static MethodInfo GetPropertyGetter(this Type type, string name) =>
            type.GetProperty(name)?.GetGetMethod()
            ?? throw new ArgumentException($"{type.DisplayName()} does not have {name} property.");

        public static bool IsIntegral(this Type type) => SmallIntegralTypes.Contains(type);

        public static bool IsCeqCompatible(this Type type) => CeqCompatibleTypes.Contains(type);

        public static bool IsBasic(this Type type) => BasicTypes.Contains(type);

        public static bool IsSealedType(this Type type) => type.IsValueType || type.IsSealed;

        public static bool IsSealedComparable(this Type type) =>
            type.ImplementsGenericInterface(typeof(IComparable<>))
            && type.IsSealedType();

        public static bool IsSealedEquatable(this Type type) =>
           type.ImplementsGenericInterface(typeof(IEquatable<>))
           && type.IsSealedType();

        public static bool IsHierarchical(this Type type) =>
            !type.IsPrimitive()
            && !type.IsNullable()
            && !typeof(IEnumerable).IsAssignableFrom(type);

        public static MethodInfo FindMethod(this Type type, string name, Type[] types)
        {
            if (type.IsInterface) {
                return type
                    .GetMethod(name, types) ?? type
                    .GetInterfaces()
                    .Select(i => FindMethod(i, name, types))
                    .FirstOrDefault(m => m != null);
            }

            return type.GetMethod(name, types);
        }
    }
}
