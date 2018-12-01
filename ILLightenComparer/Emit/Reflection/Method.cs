using System;
using System.Collections.Generic;
using System.Reflection;

namespace ILLightenComparer.Emit.Reflection
{
    internal static class Method
    {
        public delegate int StaticMethodDelegate<in T>(IContext context, T x, T y, HashSet<object> hash);

        public static readonly MethodInfo StringCompare = typeof(string).GetMethod(
            nameof(string.Compare),
            new[] { typeof(string), typeof(string), typeof(StringComparison) });

        public static readonly ConstructorInfo HashSetConstructor =
            typeof(HashSet<object>).GetConstructor(Type.EmptyTypes);

        public static MethodInfo ContextCompare =
            typeof(IContext).GetMethod(nameof(IContext.Compare));

        public static Type[] StaticCompareMethodParameters(Type objectType) => new[]
        {
            typeof(IContext),
            objectType,
            objectType,
            typeof(HashSet<object>)
        };
    }
}
