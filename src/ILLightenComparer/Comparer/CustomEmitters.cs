using System.Reflection;
using System.Reflection.Emit;
using Illuminator;
using static Illuminator.Functional;

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

        public static ILEmitter EmitCheckIfArrayLoopsAreDone(this ILEmitter il, LocalBuilder index, LocalBuilder countX, LocalBuilder countY, Label afterLoop) => il
            .AreSame(LoadLocal(index), LoadLocal(countX), out var isDoneX)
            .AreSame(LoadLocal(index), LoadLocal(countY), out var isDoneY)
            .LoadLocal(isDoneX)
            .IfFalse_S(out var checkIsDoneY)
            .LoadLocal(isDoneY)
            .IfFalse_S(out var returnM1)
            .GoTo(afterLoop)
            .MarkLabel(returnM1)
            .Return(-1)
            .MarkLabel(checkIsDoneY)
            .LoadLocal(isDoneY)
            .IfFalse_S(out var loadValues)
            .Return(1)
            .MarkLabel(loadValues);

        public static ILEmitter EmitCheckIfLoopsAreDone(this ILEmitter il, LocalBuilder xDone, LocalBuilder yDone, Label gotoNext) => il
            .LoadLocal(xDone)
            .IfFalse_S(out var checkY)
            .LoadLocal(yDone)
            .IfFalse_S(out var returnM1)
            .GoTo(gotoNext)
            .MarkLabel(returnM1)
            .Return(-1)
            .MarkLabel(checkY)
            .LoadLocal(yDone)
            .IfFalse_S(out var compare)
            .Return(1)
            .MarkLabel(compare);

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
