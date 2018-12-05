using System;
using System.Collections.Generic;
using System.Reflection;

namespace ILLightenComparer.Emit.Reflection
{
    using Set = HashSet<object>;

    internal static class Method
    {
        public delegate int StaticMethodDelegate<in T>(
            IContext context,
            T x,
            T y,
            Set xSet,
            Set ySet);

        public static readonly MethodInfo StringCompare = typeof(string).GetMethod(
            nameof(string.Compare),
            new[] { typeof(string), typeof(string), typeof(StringComparison) });

        public static readonly ConstructorInfo HashSetConstructor =
            typeof(Set).GetConstructor(Type.EmptyTypes);

        public static readonly MethodInfo HashSetAdd =
            typeof(Set).GetMethod(nameof(Set.Add), new[] { typeof(object) });

        public static MethodInfo ContextCompare =
            typeof(IContext).GetMethod(nameof(IContext.Compare));

        public static Type[] StaticCompareMethodParameters(Type objectType) => new[]
        {
            typeof(IContext),
            objectType,
            objectType,
            typeof(Set),
            typeof(Set)
        };
    }
}
