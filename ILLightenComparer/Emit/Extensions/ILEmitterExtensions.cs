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
                     .Emit(OpCodes.Brfalse, next)
                     .Emit(OpCodes.Ldloc_0)
                     .Return();
        }

        public static void EmitCheckReferenceComparison(
            this ILEmitter il,
            LocalBuilder x,
            LocalBuilder y,
            Label ifBothNull)
        {
            il.LoadLocal(x)
              .LoadLocal(y)
              .Branch(OpCodes.Bne_Un_S, out var checkX)
              .Branch(OpCodes.Br, ifBothNull)
              .MarkLabel(checkX)
              .LoadLocal(x)
              .Branch(OpCodes.Brtrue_S, out var checkY)
              .Return(-1)
              .MarkLabel(checkY)
              .LoadLocal(y)
              .Branch(OpCodes.Brtrue_S, out var next)
              .Return(1)
              .MarkLabel(next);
        }
    }
}
