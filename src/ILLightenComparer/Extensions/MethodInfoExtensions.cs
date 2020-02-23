using System;
using System.Reflection;

namespace ILLightenComparer.Extensions
{
    internal static class MethodInfoExtensions
    {
        public static TDelegate CreateDelegate<TDelegate>(this MethodInfo methodInfo)
            where TDelegate : Delegate =>
            (TDelegate)methodInfo.CreateDelegate(typeof(TDelegate));
    }
}
