using System;
using System.Reflection;

namespace ILLightenComparer.Emit.Extensions
{
    internal static class MethodBuilderExtensions
    {
        public static TDelegate CreateDelegate<TDelegate>(this MethodInfo builder)
            where TDelegate : Delegate
            => (TDelegate)builder.CreateDelegate(typeof(TDelegate));
    }
}
