using System;
using System.Reflection.Emit;
using Illuminator;

namespace ILLightenComparer.Comparer
{
    internal static class CustomEmitters
    {
        public static ILEmitter EmitReturnIfTruthy(this ILEmitter il, Label next) => il
            .Store(typeof(int), out var result)
            .LoadLocal(result)
            .IfFalse(next)
            .LoadLocal(result)
            .Return();

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

        public static ILEmitter EmitReferenceComparison(this ILEmitter il, Func<ILEmitter, ILEmitter> loadX, Func<ILEmitter, ILEmitter> loadY, Label ifEqual) => il
            .Execute(loadX)
            .Execute(loadY)
            .IfNotEqual_Un_S(out var checkX)
            .GoTo(ifEqual)
            .MarkLabel(checkX)
            .Execute(loadX)
            .IfTrue_S(out var checkY)
            .Return(-1)
            .MarkLabel(checkY)
            .Execute(loadY)
            .IfTrue_S(out var next)
            .Return(1)
            .MarkLabel(next);
    }
}
