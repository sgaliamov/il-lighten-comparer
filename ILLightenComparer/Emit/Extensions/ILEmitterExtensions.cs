using System;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Reflection;

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
                     .Return();
        }

        public static void CheckNullableValuesForNull(
            this ILEmitter il,
            LocalBuilder nullableX,
            LocalBuilder nullableY,
            Type variableType,
            Label ifBothNull)
        {
            var hasValueMethod = variableType.GetPropertyGetter(MethodName.HasValue);

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
              .Branch(OpCodes.Brtrue_S, out var getValues)
              .Return(1)
              .MarkLabel(getValues);
        }
    }
}
