using System;
using System.Reflection;

namespace ILLightenComparer.Emit.Extensions
{
    internal static class MethodInfoExtensions
    {
        public static TDelegate CreateDelegate<TDelegate>(this MethodInfo methodInfo)
            where TDelegate : Delegate
        {
            return (TDelegate)methodInfo.CreateDelegate(typeof(TDelegate));
        }
    }
}
