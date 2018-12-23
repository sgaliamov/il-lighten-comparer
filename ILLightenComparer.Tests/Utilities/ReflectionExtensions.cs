using System;
using System.Reflection;

namespace ILLightenComparer.Tests.Utilities
{
    internal static class ReflectionExtensions
    {
        public static bool IsNullable(this Type type)
        {
            return type.IsValueType
                   && type.IsGenericType
                   && !type.IsGenericTypeDefinition
                   && ReferenceEquals(type.GetGenericTypeDefinition(), typeof(Nullable<>));
        }

        public static bool IsPrimitive(this Type type)
        {
            return type.IsPrimitive
                   || type.IsEnum
                   || ReferenceEquals(type, typeof(string))
                   || ReferenceEquals(type, typeof(decimal));
        }

        public static string DisplayName(this MemberInfo memberInfo)
        {
            return $"{memberInfo}"
                .Replace("\\, ILLightenComparer.Tests\\, Version=1.0.0.0\\, Culture=neutral\\, PublicKeyToken=null", "");
        }
    }
}
