using System;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Reflection;
using ILLightenComparer.Emit.Shared;

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

        public static void CheckNullableValuesForNull(
            this ILEmitter il,
            LocalBuilder nullableX,
            LocalBuilder nullableY,
            Type nullableType,
            Label ifBothNull)
        {
            var hasValueMethod = nullableType.GetPropertyGetter(MethodName.HasValue);

            il.LoadAddress(nullableY)
              .Call(hasValueMethod)
              .Store(typeof(bool), out var secondHasValue)
              .LoadAddress(nullableX)
              .Call(hasValueMethod)
              .Branch(OpCodes.Brtrue_S, out var ifFirstHasValue)
              .LoadLocal(secondHasValue)
              .Branch(OpCodes.Brfalse_S, ifBothNull)
              .Return(-1)
              .MarkLabel(ifFirstHasValue)
              .LoadLocal(secondHasValue)
              .Branch(OpCodes.Brtrue_S, out var next)
              .Return(1)
              .MarkLabel(next);
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
