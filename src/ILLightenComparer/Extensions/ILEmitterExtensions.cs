using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Reflection;
using ILLightenComparer.Variables;
using Illuminator;

namespace ILLightenComparer.Extensions
{
    internal static class ILEmitterExtensions
    {
        public static Action<ILEmitter> Load(this IVariable variable, ushort arg) => 
            (ILEmitter il) => variable.Load(il, arg);

        public static Action<ILEmitter> LoadAddress(this IVariable variable, ushort arg) => 
            (ILEmitter il) => variable.LoadAddress(il, arg);

        public static ILEmitter EmitReturnNotZero(this ILEmitter il, Label next) =>
            il.Store(typeof(int), out var result)
              .LoadLocal(result)
              .Branch(OpCodes.Brfalse, next)
              .LoadLocal(result)
              .Return();

        public static ILEmitter EmitArgumentsReferenceComparison(this ILEmitter il) =>
            il.LoadArgument(Arg.X)
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

        public static ILEmitter EmitCheckNullablesForValue(
            this ILEmitter il,
            LocalVariableInfo nullableX,
            LocalVariableInfo nullableY,
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
            LocalVariableInfo x,
            LocalVariableInfo y,
            Label ifEqual) =>
            il.LoadLocal(x)
              .LoadLocal(y)
              .Branch(OpCodes.Bne_Un_S, out var checkX)
              .GoTo(ifEqual)
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
