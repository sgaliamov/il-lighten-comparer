using System;
using System.Reflection;
using System.Reflection.Emit;

namespace ILightenComparer.Emit.Extensions
{
    internal static class TypeBuilderExtensions
    {
        public static Func<TReturnType> EmitFactoryMethod<TReturnType>(this TypeBuilder type)
        {
            var method = type.DefineMethod(
                "GetInstance",
                MethodAttributes.Static,
                type,
                null);

            var il = method.GetILGenerator();
            il.Emit(OpCodes.Newobj, type.GetConstructor(null));
            il.Emit(OpCodes.Ret);

            return method.CreateDelegate<Func<TReturnType>>();
        }
    }
}
