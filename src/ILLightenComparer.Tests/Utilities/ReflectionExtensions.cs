using System;
using System.Reflection;

namespace ILLightenComparer.Tests.Utilities
{
    internal static class ReflectionExtensions
    {
        public static Type MakeNullable(this Type type) {
            if (!type.IsValueType) {
                throw new ArgumentException(nameof(type));
            }

            return typeof(Nullable<>).MakeGenericType(type);
        }

        public static bool IsNullable(this Type type) =>
            type.IsValueType
            && type.IsGenericType
            && !type.IsGenericTypeDefinition
            && ReferenceEquals(type.GetGenericTypeDefinition(), typeof(Nullable<>));

        public static bool IsPrimitive(this Type type) =>
            type.IsPrimitive
            || type.IsEnum
            || ReferenceEquals(type, typeof(string))
            || ReferenceEquals(type, typeof(decimal));

        public static string DisplayName(this MemberInfo memberInfo) =>
            $"{memberInfo}"
                .Replace("\\, ILLightenComparer.Tests\\, Version=1.0.0.0\\, Culture=neutral\\, PublicKeyToken=null", "");
    }
}
