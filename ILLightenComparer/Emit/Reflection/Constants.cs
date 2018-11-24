using System;
using System.Reflection;

namespace ILLightenComparer.Emit.Reflection
{
    internal static class Constants
    {
        public static readonly MethodInfo StringCompareMethod = typeof(string).GetMethod(
            nameof(string.Compare),
            new[] { typeof(string), typeof(string), typeof(StringComparison) });
    }
}
