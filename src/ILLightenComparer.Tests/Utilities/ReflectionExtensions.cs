using System;

namespace ILLightenComparer.Tests.Utilities
{
    internal static class ReflectionExtensions
    {
        public static Type MakeNullable(this Type type)
        {
            if (!type.IsValueType) {
                throw new ArgumentException(nameof(type));
            }

            return typeof(Nullable<>).MakeGenericType(type);
        }
    }
}
