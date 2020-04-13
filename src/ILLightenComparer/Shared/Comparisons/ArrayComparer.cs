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
        private readonly EmitCheckIfArrayLoopsAreDoneDelegate _emitCheckIfLoopsAreDone;

        public ArrayComparer(IResolver resolver, EmitCheckIfArrayLoopsAreDoneDelegate emitCheckIfLoopsAreDone)
        {
            _resolver = resolver;
            _emitCheckIfLoopsAreDone = emitCheckIfLoopsAreDone;
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
                _emitCheckIfLoopsAreDone(il, index, countX, countY, afterLoop);
            }

            using (il.LocalsScope()) {
                var itemVariable = new ArrayItemVariable(arrayType, ownerType, xArray, yArray, index);

                var itemComparison = _resolver.GetComparisonEmitter(itemVariable);
                itemComparison.Emit(il, continueLoop);
                itemComparison.EmitCheckForIntermediateResult(il, continueLoop);

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
    }
}
