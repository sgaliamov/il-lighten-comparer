using System;
using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Extensions;
using ILLightenComparer.Variables;
using Illuminator;
using static Illuminator.Functional;

namespace ILLightenComparer.Shared.Comparisons
{
    internal sealed class ArrayComparer
    {
        private const string LengthMethodName = nameof(Array.Length);
        private readonly IResolver _resolver;

        public ArrayComparer(IResolver resolver) => _resolver = resolver;

        public ILEmitter Compare(
            Type arrayType,
            Type ownerType,
            LocalBuilder xArray,
            LocalBuilder yArray,
            LocalBuilder countX,
            LocalBuilder countY,
            ILEmitter il,
            Label afterLoop)
        {
            il.LoadInteger(0)
              .Store(typeof(int), out var index)
              .DefineLabel(out var loopStart)
              .DefineLabel(out var continueLoop)
              .MarkLabel(loopStart);

            using (il.LocalsScope()) {
                EmitCheckIfLoopsAreDone(index, countX, countY, il, afterLoop);
            }

            using (il.LocalsScope()) {
                var itemVariable = new ArrayItemVariable(arrayType, ownerType, xArray, yArray, index);

                var itemComparison = _resolver.GetComparisonEmitter(itemVariable);
                itemComparison.Emit(il, continueLoop);

                if (itemComparison.PutsResultInStack) {
                    il.EmitReturnIfTruthy(continueLoop);
                }

                return il.MarkLabel(continueLoop)
                         .Add(LoadLocal(index), LoadInteger(1))
                         .Store(index)
                         .GoTo(loopStart);
            }
        }

        public (LocalBuilder countX, LocalBuilder countY) EmitLoadCounts(
            Type arrayType,
            LocalBuilder arrayX,
            LocalBuilder arrayY,
            ILEmitter il)
        {
            il.LoadLocal(arrayX)
              .Call(arrayType.GetPropertyGetter(LengthMethodName))
              .Store(typeof(int), out var countX)
              .LoadLocal(arrayY)
              .Call(arrayType.GetPropertyGetter(LengthMethodName))
              .Store(typeof(int), out var countY);

            return (countX, countY);
        }

        private static void EmitCheckIfLoopsAreDone(
            LocalBuilder index,
            LocalBuilder countX,
            LocalBuilder countY,
            ILEmitter il,
            Label afterLoop)
        {
            il.AreSame(LoadLocal(index), LoadLocal(countX), out var isDoneX)
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
}
