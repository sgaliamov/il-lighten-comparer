using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters;

namespace ILLightenComparer.Emit.Extensions
{
    internal static class ILEmitterExtensions
    {
        public static ILEmitter EmitReturnNotZero(this ILEmitter il, Label next)
        {
            return il.Emit(OpCodes.Stloc_0)
                     .Emit(OpCodes.Ldloc_0)
                     .Emit(OpCodes.Brfalse_S, next)
                     .Emit(OpCodes.Ldloc_0)
                     .Return()
                     .MarkLabel(next);
        }
    }
}
