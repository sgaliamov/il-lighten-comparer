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

        public static ILEmitter EmitCheckIfLoopsAreDone(this ILEmitter il, LocalBuilder index, LocalBuilder countX, LocalBuilder countY, Label afterLoop) => il
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
    }
}
