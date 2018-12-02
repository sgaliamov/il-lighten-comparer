using System;

namespace ILLightenComparer.Tests.Utilities
{
    internal static class Extensions
    {
        public static bool IsNullable(this Type type) =>
            type != null
            && type.IsGenericType
            && !type.IsGenericTypeDefinition
            && ReferenceEquals(type.GetGenericTypeDefinition(), typeof(Nullable<>));

        public static bool IsPrimitive(this Type type) =>
            type.IsPrimitive
            || type.IsEnum
            || ReferenceEquals(type, typeof(string))
            || ReferenceEquals(type, typeof(decimal));
    }
}
