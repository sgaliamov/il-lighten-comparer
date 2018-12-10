using System;
using System.Reflection;
using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit.Reflection
{
    internal static class Method
    {
        public delegate int StaticMethodDelegate<in T>(
            IComparerContext context,
            T x,
            T y,
            ConcurrentSet<object> xSet,
            ConcurrentSet<object> ySet);

        public static readonly MethodInfo StringCompare = typeof(string).GetMethod(
            nameof(string.Compare),
            new[] { typeof(string), typeof(string), typeof(StringComparison) });

        public static readonly ConstructorInfo SetConstructor =
            typeof(ConcurrentSet<object>).GetConstructor(Type.EmptyTypes);

        public static readonly MethodInfo SetAdd =
            typeof(ConcurrentSet<object>).GetMethod(nameof(ConcurrentSet<object>.TryAdd), new[] { typeof(object), typeof(byte) });

        public static readonly MethodInfo SetGetCount =
            typeof(ConcurrentSet<object>).GetProperty(nameof(ConcurrentSet<object>.Count))?.GetGetMethod();

        public static MethodInfo ContextCompare =
            typeof(IComparerContext).GetMethod(nameof(IComparerContext.DelayedCompare));

        public static Type[] StaticCompareMethodParameters(Type objectType) => new[]
        {
            typeof(IComparerContext),
            objectType,
            objectType,
            typeof(ConcurrentSet<object>),
            typeof(ConcurrentSet<object>)
        };
    }
}
