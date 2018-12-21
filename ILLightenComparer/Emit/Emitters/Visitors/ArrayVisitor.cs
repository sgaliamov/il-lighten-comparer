using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Comparisons;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.Emitters.Visitors
{
    internal sealed class ArrayVisitor : CollectionVisitor
    {
        private const int LocalX = Arg.X; // 1
        private const int LocalY = Arg.Y; // 2
        private const int LocalCountX = 3;
        private const int LocalCountY = 4;
        private const int LocalDoneX = 5;
        private const int LocalDoneY = 6;
        private const int LocalIndex = 7;

        private readonly ComparerContext _context;
        private readonly Converter _converter;
        private readonly VariableLoader _loader;

        public ArrayVisitor(
            ComparerContext context,
            StackVisitor stackVisitor,
            CompareVisitor compareVisitor,
            VariableLoader loader,
            Converter converter)
            : base(stackVisitor, compareVisitor, loader)
        {
            _context = context;
            _loader = loader;
            _converter = converter;
        }

        public ILEmitter Visit(ArrayComparison comparison, ILEmitter il)
        {
            var (x, y, gotoNext) = EmitLoad(il, comparison);

            var (countX, countY) = EmitLoadCounts(il, comparison, x, y);

            EmitCheckForNegativeCount(il, countX, countY, comparison.Variable.VariableType);

            var variable = comparison.Variable;
            if (_context.GetConfiguration(variable.OwnerType).IgnoreCollectionOrder)
            {
                EmitArraySorting(il, variable.VariableType, x, y);
            }

            Loop(il, variable, x, y, countX, countY, gotoNext);

            return il.MarkLabel(gotoNext);
        }

        private void Loop(
            ILEmitter il,
            IVariable variable,
            LocalBuilder xArray,
            LocalBuilder yArray,
            LocalBuilder countX,
            LocalBuilder countY,
            Label gotoNextMember)
        {
            il.LoadConstant(0)
              .Store(typeof(int), LocalIndex, out var index)
              .DefineLabel(out var loopStart)
              .DefineLabel(out var continueLoop)
              .MarkLabel(loopStart);

            EmitCheckIfLoopsAreDone(il, index, countX, countY, gotoNextMember);

            var elementType = variable.VariableType.GetElementType();
            if (elementType.IsNullable())
            {
                var arrayItemVariable = new ArrayItemVariable(variable.VariableType, variable.OwnerType, xArray, yArray, index);
                arrayItemVariable.Load(_loader, il, Arg.X);
                il.Store(elementType, LocalX, out var nullableX);
                arrayItemVariable.Load(_loader, il, Arg.Y);
                il.Store(elementType, LocalY, out var nullableY);
                il.CheckNullableValuesForNull(nullableX, nullableY, elementType, continueLoop);

                var itemComparison = _converter.CreateNullableVariableComparison(arrayItemVariable, nullableX, nullableY);

                Visit(il, itemComparison, continueLoop);
            }
            else
            {
                var itemComparison = _converter.CreateArrayItemComparison(variable, xArray, yArray, index);

                Visit(il, itemComparison, continueLoop);
            }

            il.MarkLabel(continueLoop)
              .LoadLocal(index)
              .LoadConstant(1)
              .Emit(OpCodes.Add)
              .Store(index)
              .Branch(OpCodes.Br, loopStart);
        }

        private static void EmitCheckIfLoopsAreDone(
            ILEmitter il,
            LocalBuilder index,
            LocalBuilder countX,
            LocalBuilder countY,
            Label gotoNextMember)
        {
            il.LoadLocal(index)
              .LoadLocal(countX)
              .Emit(OpCodes.Ceq)
              .Store(typeof(int), LocalDoneX, out var isDoneX)
              .LoadLocal(index)
              .LoadLocal(countY)
              .Emit(OpCodes.Ceq)
              .Store(typeof(int), LocalDoneY, out var isDoneY)
              .LoadLocal(isDoneX)
              .Branch(OpCodes.Brfalse_S, out var checkIsDoneY)
              .LoadLocal(isDoneY)
              .Branch(OpCodes.Brfalse_S, out var returnM1)
              .Branch(OpCodes.Br, gotoNextMember)
              .MarkLabel(returnM1)
              .Return(-1)
              .MarkLabel(checkIsDoneY)
              .LoadLocal(isDoneY)
              .Branch(OpCodes.Brfalse_S, out var loadValues)
              .Return(1)
              .MarkLabel(loadValues);
        }

        private static (LocalBuilder countX, LocalBuilder countY) EmitLoadCounts(
            ILEmitter il,
            ArrayComparison comparison,
            LocalBuilder x,
            LocalBuilder y)
        {
            il.LoadLocal(x)
              .Call(comparison.GetLengthMethod)
              .Store(typeof(int), LocalCountX, out var countX)
              .LoadLocal(y)
              .Call(comparison.GetLengthMethod)
              .Store(typeof(int), LocalCountY, out var countY);

            return (countX, countY);
        }

        private static void EmitCheckForNegativeCount(
            ILEmitter il,
            LocalBuilder countX,
            LocalBuilder countY,
            MemberInfo memberType)
        {
            il.LoadConstant(0)
              .LoadLocal(countX)
              .Branch(OpCodes.Bgt_S, out var negativeException)
              .LoadConstant(0)
              .LoadLocal(countY)
              .Branch(OpCodes.Ble_S, out var loopInit)
              .MarkLabel(negativeException)
              .LoadString($"Collection {memberType.DisplayName()} has negative count of elements.")
              .Emit(OpCodes.Newobj, typeof(IndexOutOfRangeException).GetConstructor(new[] { typeof(string) }))
              .Emit(OpCodes.Throw)
              .MarkLabel(loopInit);
        }
    }
}
