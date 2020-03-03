using System;
using System.Reflection.Emit;
using ILLightenComparer.Emitters.Variables;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using Illuminator;

namespace ILLightenComparer.Emitters.Visitors.Collection
{
    internal sealed class ArrayComparer
    {
        private readonly CompareVisitor _compareVisitor;
        private readonly ComparisonsProvider _comparisons;

        public ArrayComparer(CompareVisitor compareVisitor, ComparisonsProvider comparisons)
        {
            _compareVisitor = compareVisitor;
            _comparisons = comparisons;
        }

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

                var itemComparison = _comparisons.GetComparison(itemVariable);
                itemComparison.Accept(_compareVisitor, il, continueLoop);

                if (itemComparison.PutsResultInStack) {
                    il.EmitReturnNotZero(continueLoop);
                }

                return il.MarkLabel(continueLoop)
                         .Add(il => il.LoadLocal(index), il => il.LoadInteger(1))
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
              .Call(arrayType.GetPropertyGetter(MethodName.Length))
              .Store(typeof(int), out var countX)
              .LoadLocal(arrayY)
              .Call(arrayType.GetPropertyGetter(MethodName.Length))
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
            il.AreSame(il => il.LoadLocal(index), il => il.LoadLocal(countX), out var isDoneX)
              .AreSame(il => il.LoadLocal(index), il => il.LoadLocal(countY), out var isDoneY)
              .LoadLocal(isDoneX)
              .Branch(OpCodes.Brfalse_S, out var checkIsDoneY)
              .LoadLocal(isDoneY)
              .Branch(OpCodes.Brfalse_S, out var returnM1)
              .GoTo(afterLoop)
              .MarkLabel(returnM1)
              .Return(-1)
              .MarkLabel(checkIsDoneY)
              .LoadLocal(isDoneY)
              .Branch(OpCodes.Brfalse_S, out var loadValues)
              .Return(1)
              .MarkLabel(loadValues);
        }
    }
}
