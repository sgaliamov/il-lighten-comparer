using System.Reflection;
using System.Reflection.Emit;
using Illuminator;

namespace ILLightenComparer.Equality
{
    internal static class CustomEmitters
    {
        public static ILEmitter EmitReturnIfFalsy(this ILEmitter il, Label next) => il.IfTrue(next).Return(0);

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

        public static ILEmitter EmitReferenceComparison(this ILEmitter il, LocalVariableInfo x, LocalVariableInfo y, Label ifEqual) => il
            .LoadLocal(x)
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
