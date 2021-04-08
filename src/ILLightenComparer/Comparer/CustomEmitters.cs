using System;
using System.Reflection.Emit;
using ILLightenComparer.Extensions;
using Illuminator;
using static ILLightenComparer.Extensions.Functions;

namespace ILLightenComparer.Comparer
{
    internal static class CustomEmitters
    {
        /// <summary>
        /// Returns non zero value if stack has it, otherwise got to <paramref name="next"/>.
        /// </summary>
        public static ILEmitter EmitReturnIfTruthy(this ILEmitter il, Label next) => il
            .Stloc(typeof(int), out var result)
            .Ldloc(result)
            .IfFalse(next)
            .Ret(Ldloc(result));

        public static ILEmitter EmitCheckIfLoopsAreDone(this ILEmitter il, LocalBuilder isDoneX, LocalBuilder isDoneY, Label gotoNext) => il
            .Ldloc(isDoneX)
            .IfFalse_S(out var checkIsDoneY)
            .Ldloc(isDoneY)
            .IfFalse_S(out var returnM1)
            .GoTo(gotoNext)
            .MarkLabel(returnM1)
            .Ret(-1)
            .MarkLabel(checkIsDoneY)
            .LoadLocal(isDoneY)
            .IfFalse_S(out var compare)
            .Ret(1)
            .MarkLabel(compare);

        public static ILEmitter EmitReferenceComparison(this ILEmitter il, ILEmitterFunc loadX, ILEmitterFunc loadY, ILEmitterFunc ifEqual) => il
            .Bne_Un_S(loadX, loadY, out var checkX)
            .Emit(ifEqual)
            .MarkLabel(checkX)
            .Execute(loadX)
            .IfTrue_S(out var checkY)
            .Ret(-1)
            .MarkLabel(checkY)
            .Execute(loadY)
            .IfTrue_S(out var next)
            .Ret(1)
            .MarkLabel(next);

        public static ILEmitter EmitCheckNullablesForValue(this ILEmitter il, ILEmitterFunc nullableX, ILEmitterFunc nullableY, Type nullableType, Label ifBothNull)
        {
            var hasValueMethod = nullableType.GetPropertyGetter("HasValue");

            return il.Emit(nullableY)
                     .Call(hasValueMethod)
                     .Stloc(typeof(bool), out var secondHasValue)
                     .Emit(nullableX)
                     .Call(hasValueMethod)
                     .IfTrue_S(out var ifFirstHasValue)
                     .LoadLocal(secondHasValue)
                     .IfFalse(ifBothNull)
                     .Ret(-1)
                     .MarkLabel(ifFirstHasValue)
                     .LoadLocal(secondHasValue)
                     .IfTrue_S(out var next)
                     .Ret(1)
                     .MarkLabel(next);
        }
    }
}
