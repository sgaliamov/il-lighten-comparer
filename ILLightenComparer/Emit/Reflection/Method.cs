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
            ObjectsSet xSet,
            ObjectsSet ySet);

        public static readonly MethodInfo StringCompare = typeof(string).GetMethod(
            nameof(string.Compare),
            new[] { typeof(string), typeof(string), typeof(StringComparison) });

        public static readonly ConstructorInfo SetConstructor =
            typeof(ObjectsSet).GetConstructor(Type.EmptyTypes);

        public static readonly MethodInfo SetAdd =
            typeof(ObjectsSet).GetMethod(nameof(ObjectsSet.TryAdd), new[] { typeof(object), typeof(byte) });

        public static readonly MethodInfo SetGetCount =
            typeof(ObjectsSet).GetProperty(nameof(ObjectsSet.Count))?.GetGetMethod();

        public static MethodInfo ContextCompare =
            typeof(IComparerContext).GetMethod(nameof(IComparerContext.Compare));

        public static Type[] StaticCompareMethodParameters(Type objectType) => new[]
        {
            typeof(IComparerContext),
            objectType,
            objectType,
            typeof(ObjectsSet),
            typeof(ObjectsSet)
        };
    }
}
