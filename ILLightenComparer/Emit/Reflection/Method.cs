using System;
using System.Reflection;

namespace ILLightenComparer.Emit.Reflection
{
    internal static class Method
    {
        public static readonly MethodInfo StringCompare = typeof(string).GetMethod(
            nameof(string.Compare),
            new[] { typeof(string), typeof(string), typeof(StringComparison) });
    }
}
