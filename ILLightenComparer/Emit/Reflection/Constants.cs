using System;
using System.Collections;
using System.Reflection;

namespace ILLightenComparer.Emit.Reflection
{
    internal static class Constants
    {
        public const string FactoryMethodName = "CreateInstance";
        public const string CompareMethodName = nameof(IComparer.Compare);

        public static readonly MethodInfo StringCompareMethod = typeof(string).GetMethod(
            nameof(string.Compare),
            new[] { typeof(string), typeof(string), typeof(StringComparison) });
    }
}
