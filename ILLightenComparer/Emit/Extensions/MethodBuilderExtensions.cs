using System;
using System.Reflection.Emit;

namespace ILLightenComparer.Emit.Extensions
{
    internal static class MethodBuilderExtensions
    {
        public static TDelegate CreateDelegate<TDelegate>(this MethodBuilder builder)
            where TDelegate : Delegate
            => (TDelegate)builder.CreateDelegate(typeof(TDelegate));
    }
}
