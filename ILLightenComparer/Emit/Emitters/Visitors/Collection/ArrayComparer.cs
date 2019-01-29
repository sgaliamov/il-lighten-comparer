using System;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;
using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit.Emitters.Visitors.Collection
{
    internal sealed class ArrayComparer
    {
        private readonly CompareVisitor _compareVisitor;
        private readonly Converter _converter;

        public ArrayComparer(CompareVisitor compareVisitor, Converter converter)
        {
            _compareVisitor = compareVisitor;
            _converter = converter;
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
            il.LoadConstant(0)
              .Store(typeof(int), out var index)
              .DefineLabel(out var loopStart)
              .DefineLabel(out var continueLoop)
              .MarkLabel(loopStart);

            EmitCheckIfLoopsAreDone(index, countX, countY, il, afterLoop);

            var itemVariable = new ArrayItemVariable(arrayType, ownerType, xArray, yArray, index);

            return _converter.CreateComparison(itemVariable)
                             .Accept(_compareVisitor, il, continueLoop)
                             .EmitReturnNotZero(continueLoop)
                             .MarkLabel(continueLoop)
                             .LoadLocal(index)
                             .LoadConstant(1)
                             .Emit(OpCodes.Add)
                             .Store(index)
                             .Branch(OpCodes.Br, loopStart);
        }

        public (LocalBuilder countX, LocalBuilder countY) EmitLoadCounts(Type arrayType, LocalBuilder arrayX, LocalBuilder arrayY, ILEmitter il)
        {
            il.LoadLocal(arrayX)
              .Emit(OpCodes.Call, arrayType.GetPropertyGetter(MethodName.Length))
              .Store(typeof(int), out var countX)
              .LoadLocal(arrayY)
              .Emit(OpCodes.Call, arrayType.GetPropertyGetter(MethodName.Length))
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
            il.LoadLocal(index)
              .LoadLocal(countX)
              .Emit(OpCodes.Ceq)
              .Store(typeof(int), out var isDoneX)
              .LoadLocal(index)
              .LoadLocal(countY)
              .Emit(OpCodes.Ceq)
              .Store(typeof(int), out var isDoneY)
              .LoadLocal(isDoneX)
              .Branch(OpCodes.Brfalse_S, out var checkIsDoneY)
              .LoadLocal(isDoneY)
              .Branch(OpCodes.Brfalse_S, out var returnM1)
              .Branch(OpCodes.Br, afterLoop)
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
