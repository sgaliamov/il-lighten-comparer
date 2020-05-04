using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Extensions;
using Illuminator;
using static Illuminator.Functional;

namespace ILLightenComparer.Comparer
{
    internal static class CustomEmitters
    {
        /// <summary>
        /// Returns non zero value if stack has it, otherwise got to <paramref name="next"/>.
        /// </summary>
        public static ILEmitter EmitReturnIfTruthy(this ILEmitter il, Label next) => il
            .Store(typeof(int), out var result)
            .LoadLocal(result)
            .IfFalse(next)
            .Return(LoadLocal(result));

        public static ILEmitter EmitCheckIfLoopsAreDone(this ILEmitter il, LocalBuilder isDoneX, LocalBuilder isDoneY, Label gotoNext) => il
            .LoadLocal(isDoneX)
            .IfFalse_S(out var checkIsDoneY)
            .LoadLocal(isDoneY)
            .IfFalse_S(out var returnM1)
            .GoTo(gotoNext)
            .MarkLabel(returnM1)
            .Return(-1)
            .MarkLabel(checkIsDoneY)
            .LoadLocal(isDoneY)
            .IfFalse_S(out var compare)
            .Return(1)
            .MarkLabel(compare);

        public static ILEmitter EmitReferenceComparison(this ILEmitter il, ILEmitterFunc loadX, ILEmitterFunc loadY, ILEmitterFunc ifEqual) => il
            .IfNotEqual_Un_S(loadX, loadY, out var checkX)
            .Execute(ifEqual)
            .MarkLabel(checkX)
            .Execute(loadX)
            .IfTrue_S(out var checkY)
            .Return(-1)
            .MarkLabel(checkY)
            .Execute(loadY)
            .IfTrue_S(out var next)
            .Return(1)
            .MarkLabel(next);

        public static ILEmitter EmitCheckNullablesForValue(ILEmitter il, LocalVariableInfo nullableX, LocalVariableInfo nullableY, Type nullableType, Label ifBothNull)
        {
            var hasValueMethod = nullableType.GetPropertyGetter("HasValue");

            return il.LoadAddress(nullableY)
                     .Call(hasValueMethod)
                     .Store(typeof(bool), out var secondHasValue)
                     .LoadAddress(nullableX)
                     .Call(hasValueMethod)
                     .IfTrue_S(out var ifFirstHasValue)
                     .LoadLocal(secondHasValue)
                     .IfFalse(ifBothNull)
                     .Return(-1)
                     .MarkLabel(ifFirstHasValue)
                     .LoadLocal(secondHasValue)
                     .IfTrue_S(out var next)
                     .Return(1)
                     .MarkLabel(next);
        }
    }
}
