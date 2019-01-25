using System;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Reflection;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2;

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

        public static ILEmitter EmitArgumentsReferenceComparison(this ILEmitter il)
        {
            return il.LoadArgument(Arg.X)
                     .LoadArgument(Arg.Y)
                     .Branch(OpCodes.Bne_Un_S, out var checkY)
                     .Return(0)
                     .MarkLabel(checkY)
                     .LoadArgument(Arg.Y)
                     .Branch(OpCodes.Brtrue_S, out var checkX)
                     .Return(1)
                     .MarkLabel(checkX)
                     .LoadArgument(Arg.X)
                     .Branch(OpCodes.Brtrue_S, out var next)
                     .Return(-1)
                     .MarkLabel(next);
        }

        public static ILEmitter EmitCheckNullablesForValue(
            this ILEmitter il,
            LocalBuilder nullableX,
            LocalBuilder nullableY,
            Type nullableType,
            Label ifBothNull)
        {
            var hasValueMethod = nullableType.GetPropertyGetter(MethodName.HasValue);

            return il.LoadAddress(nullableY)
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

        public static ILEmitter EmitReferenceComparison(
            this ILEmitter il,
            LocalBuilder x,
            LocalBuilder y,
            Label ifEqual)
        {
            return il.LoadLocal(x)
                     .LoadLocal(y)
                     .Branch(OpCodes.Bne_Un_S, out var checkX)
                     .Branch(OpCodes.Br, ifEqual)
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
