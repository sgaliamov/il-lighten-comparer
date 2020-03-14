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
        public static ILEmitter EmitReturnNotZero(this ILEmitter il, Label next) =>
            il.Store(typeof(int), out var result)
              .LoadLocal(result)
              .IfFalse(next)
              .LoadLocal(result)
              .Return();

        public static ILEmitter EmitArgumentsReferenceComparison(this ILEmitter il) =>
            il.LoadArgument(Arg.X)
              .LoadArgument(Arg.Y)
              .IfNotEqual_Un_S(out var checkY)
              .Return(0)
              .MarkLabel(checkY)
              .LoadArgument(Arg.Y)
              .IfTrue_S(out var checkX)
              .Return(1)
              .MarkLabel(checkX)
              .LoadArgument(Arg.X)
              .IfTrue_S(out var next)
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
                     .IfTrue_S(out var ifFirstHasValue)
                     .LoadLocal(secondHasValue)
                     .IfFalse_S(ifBothNull)
                     .Return(-1)
                     .MarkLabel(ifFirstHasValue)
                     .LoadLocal(secondHasValue)
                     .IfTrue_S(out var next)
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
              .IfNotEqual_Un_S(out var checkX)
              .GoTo(ifEqual)
              .MarkLabel(checkX)
              .LoadLocal(x)
              .IfTrue_S(out var checkY)
              .Return(-1)
              .MarkLabel(checkY)
              .LoadLocal(y)
              .IfTrue_S(out var next)
              .Return(1)
              .MarkLabel(next);
    }
}
