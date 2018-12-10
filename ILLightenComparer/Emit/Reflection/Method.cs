using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace ILLightenComparer.Emit.Reflection
{
    using Set = ConcurrentDictionary<object, byte>;

    internal static class Method
    {
        public delegate int StaticMethodDelegate<in T>(
            IComparerContext context,
            T x,
            T y,
            Set xSet,
            Set ySet);

        public static readonly MethodInfo StringCompare = typeof(string).GetMethod(
            nameof(string.Compare),
            new[] { typeof(string), typeof(string), typeof(StringComparison) });

        public static readonly ConstructorInfo SetConstructor =
            typeof(Set).GetConstructor(Type.EmptyTypes);

        public static readonly MethodInfo SetAdd =
            typeof(Set).GetMethod(nameof(Set.TryAdd), new[] { typeof(object), typeof(byte) });

        public static readonly MethodInfo SetGetCount =
            typeof(Set).GetProperty(nameof(Set.Count))?.GetGetMethod();

        public static MethodInfo ContextCompare =
            typeof(IComparerContext).GetMethod(nameof(IComparerContext.Compare));

        public static Type[] StaticCompareMethodParameters(Type objectType) => new[]
        {
            typeof(IComparerContext),
            objectType,
            objectType,
            typeof(Set),
            typeof(Set)
        };
    }
}
