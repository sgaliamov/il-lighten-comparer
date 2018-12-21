using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Comparisons;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Emitters.Visitors
{
    internal sealed class ArrayVisitor
    {
        private const int LocalX = Arg.X; // 1
        private const int LocalY = Arg.Y; // 2
        private const int LocalCountX = 3;
        private const int LocalCountY = 4;
        private const int LocalDoneX = 5;
        private const int LocalDoneY = 6;
        private const int LocalNullableX = 7;
        private const int LocalNullableY = 8;
        private const int LocalIndex = 9;

        private readonly CompareVisitor _compareVisitor;
        private readonly ComparerContext _context;
        private readonly Converter _converter;
        private readonly VariableLoader _loader;
        private readonly StackVisitor _stackVisitor;

        public ArrayVisitor(
            ComparerContext context,
            StackVisitor stackVisitor,
            CompareVisitor compareVisitor,
            VariableLoader loader,
            Converter converter)
        {
            _context = context;
            _compareVisitor = compareVisitor;
            _loader = loader;
            _converter = converter;
            _stackVisitor = stackVisitor;
        }

        public ILEmitter Visit(ArrayComparison comparison, ILEmitter il)
        {
            il.DefineLabel(out var gotoNextMember);

            var variable = comparison.Variable;
            variable.Load(_loader, il, Arg.X).Store(variable.VariableType, LocalX, out var x);
            variable.Load(_loader, il, Arg.Y).Store(variable.VariableType, LocalY, out var y);

            il.EmitCheckReferenceComparison(x, y, gotoNextMember);

            var (countX, countY) = EmitLoadCounts(il, comparison, x, y);

            EmitCheckForNegativeCount(il, countX, countY, comparison.Variable.VariableType);

            if (_context.GetConfiguration(variable.OwnerType).IgnoreCollectionOrder)
            {
                EmitCollectionsSorting(il, variable.VariableType, x, y);
            }

            Loop(il, variable, x, y, countX, countY, gotoNextMember);

            return il.MarkLabel(gotoNextMember);
        }

        private static void EmitCollectionsSorting(
            ILEmitter il,
            Type arrayType,
            LocalBuilder xArray,
            LocalBuilder yArray)
        {
            var elementType = arrayType.GetElementType();
            if (elementType.GetUnderlyingType().ImplementsGeneric(typeof(IComparable<>)))
            {
                EmitSortArray(il, elementType, xArray);
                EmitSortArray(il, elementType, yArray);
            }
            else
            {
                var getComparerMethod = Method.GetComparer.MakeGenericMethod(elementType);

                il.LoadArgument(Arg.Context)
                  .Emit(OpCodes.Call, getComparerMethod)
                  .Store(getComparerMethod.ReturnType, out var comparer);

                EmitSortArray(il, elementType, xArray, comparer);
                EmitSortArray(il, elementType, yArray, comparer);
            }
        }

        private static void EmitSortArray(ILEmitter il, Type elementType, LocalBuilder array, LocalBuilder comparer)
        {
            var copyMethod = Method.ToArray.MakeGenericMethod(elementType);
            var sortMethod = Method.GetArraySortWithComparer(elementType);

            il.LoadLocal(array)
              .Call(copyMethod)
              .Store(array)
              .LoadLocal(array)
              .LoadLocal(comparer)
              .Call(sortMethod);
        }

        private static void EmitSortArray(
            ILEmitter il,
            Type elementType,
            LocalBuilder array)
        {
            var copyMethod = Method.ToArray.MakeGenericMethod(elementType);
            var sortMethod = Method.GetArraySort(elementType);

            il.LoadLocal(array)
              .Call(copyMethod)
              .Store(array)
              .LoadLocal(array)
              .Call(sortMethod);
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
                il.Store(elementType, LocalNullableX, out var nullableX);
                arrayItemVariable.Load(_loader, il, Arg.Y);
                il.Store(elementType, LocalNullableY, out var nullableY);
                il.CheckNullableValuesForNull(nullableX, nullableY, elementType, continueLoop);

                var itemComparison = _converter.CreateNullableVariableComparison(arrayItemVariable, nullableX, nullableY);

                Visit(il, itemComparison, continueLoop, gotoNextMember);
            }
            else
            {
                var itemComparison = _converter.CreateArrayItemComparison(variable, xArray, yArray, index);

                Visit(il, itemComparison, continueLoop, gotoNextMember);
            }

            il.MarkLabel(continueLoop)
              .LoadLocal(index)
              .LoadConstant(1)
              .Emit(OpCodes.Add)
              .Store(index)
              .Branch(OpCodes.Br, loopStart);
        }

        private void Visit(ILEmitter il, IComparisonAcceptor itemComparison, Label continueLoop, Label gotoNext)
        {
            itemComparison.LoadVariables(_stackVisitor, il, gotoNext);
            itemComparison.Accept(_compareVisitor, il)
                          .EmitReturnNotZero(continueLoop);
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
