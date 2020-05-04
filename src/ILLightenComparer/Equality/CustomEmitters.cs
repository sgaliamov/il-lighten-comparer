using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Extensions;
using Illuminator;

namespace ILLightenComparer.Equality
{
    internal static class CustomEmitters
    {
        /// <summary>
        /// Returns zero if stack has zero value, otherwise got to <paramref name="next"/>.
        /// </summary>
        public static ILEmitter EmitReturnIfFalsy(this ILEmitter il, Label next) => il
            .IfTrue(next)
            .Return(0);

        public static ILEmitter EmitCheckIfLoopsAreDone(this ILEmitter il, LocalBuilder isDoneX, LocalBuilder isDoneY, Label gotoNext) => il
            .LoadLocal(isDoneX)
            .IfFalse_S(out var checkIsDoneY)
            .LoadLocal(isDoneY)
            .IfFalse_S(out var returnFalse)
            .GoTo(gotoNext)
            .MarkLabel(returnFalse)
            .Return(0)
            .MarkLabel(checkIsDoneY)
            .LoadLocal(isDoneY)
            .IfFalse_S(out var next)
            .Return(0)
            .MarkLabel(next);

        public static ILEmitter EmitReferenceComparison(this ILEmitter il, ILEmitterFunc loadX, ILEmitterFunc loadY, ILEmitterFunc ifEqual) => il
            .IfNotEqual_Un_S(loadX, loadY, out var checkX)
            .Execute(ifEqual)
            .MarkLabel(checkX)
            .Execute(loadX)
            .IfTrue_S(out var checkY)
            .Return(0)
            .MarkLabel(checkY)
            .Execute(loadY)
            .IfTrue_S(out var next)
            .Return(0)
            .MarkLabel(next);

        public static ILEmitter EmitCheckNullablesForValue(this ILEmitter il, LocalVariableInfo nullableX, LocalVariableInfo nullableY, Type nullableType, Label ifBothNull)
        {
            var hasValueMethod = nullableType.GetPropertyGetter("HasValue");

            return il.LoadAddress(nullableY)
                     .Call(hasValueMethod)
                     .Store(typeof(bool), out var secondHasValue)
                     .LoadAddress(nullableX)
                     .Call(hasValueMethod)
                     .IfTrue_S(out var ifFirstHasValue)
                     .LoadLocal(secondHasValue)
                     .IfFalse_S(ifBothNull)
                     .Return(0)
                     .MarkLabel(ifFirstHasValue)
                     .LoadLocal(secondHasValue)
                     .IfTrue_S(out var next)
                     .Return(0)
                     .MarkLabel(next);
        }
    }
}
