using System;
using System.Reflection.Emit;

namespace ILightenComparer.Emit.Extensions
{
    internal static class MethodBuilderExtensions
    {
        public static TDelegate CreateDelegate<TDelegate>(this MethodBuilder builder)
            where TDelegate : Delegate
            => (TDelegate)builder.CreateDelegate(typeof(TDelegate));
    }
}
