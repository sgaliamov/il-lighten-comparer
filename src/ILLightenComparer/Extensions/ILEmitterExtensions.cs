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
