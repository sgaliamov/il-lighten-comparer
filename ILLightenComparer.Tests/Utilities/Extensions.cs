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
    }
}
