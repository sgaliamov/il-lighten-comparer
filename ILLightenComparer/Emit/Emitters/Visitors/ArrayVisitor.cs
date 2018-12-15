using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Comparisons;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Emitters.Visitors
{
    internal sealed class ArrayVisitor
    {
        private const int LocalX = 1;
        private const int LocalY = 2;
        private const int LocalCountX = 3;
        private const int LocalCountY = 4;
        private const int LocalDoneX = 5;
        private const int LocalDoneY = 6;
        private const int LocalIndex = 7;
        private readonly VariableLoader _loader;

        private readonly CompareVisitor _visitor;

        public ArrayVisitor(VariableLoader loader, CompareVisitor visitor)
        {
            _visitor = visitor;
            _loader = loader;
        }

        public ILEmitter Visit(CollectionComparison comparison, ILEmitter il)
        {
            var variable = comparison.Variable;
            il.DefineLabel(out var gotoNextMember);

            variable.Load(_loader, il, Arg.X).Store(variable.VariableType, LocalX, out var x);
            variable.Load(_loader, il, Arg.Y).Store(variable.VariableType, LocalY, out var y);

            EmitCheckMemberReferenceComparison(il, x, y, gotoNextMember);

            var (countX, countY) = EmitLoadCounts(il, comparison, x, y);

            il.LoadConstant(0)
              .Store(typeof(int), LocalIndex, out var index)
              .DefineLabel(out var loopStart)
              .MarkLabel(loopStart);

            EmitCheckIfLoopsAreDone(il, index, countX, countY, gotoNextMember);

            if (comparison.ElementType.IsNullable())
            {
                EmitLoadNullableValues(il, comparison, x, y, index, gotoNextMember);
            }
            else
            {
                EmitLoadValues(il, comparison, x, y, index);
            }

            comparison.Accept(_visitor, il)
                      .DefineLabel(out var continueLoop)
                      .EmitReturnNotZero(continueLoop);

            il.MarkLabel(continueLoop)
              .LoadLocal(index)
              .LoadConstant(LocalX)
              .Emit(OpCodes.Add)
              .Store(index)
              .Branch(OpCodes.Br, loopStart)
              .MarkLabel(gotoNextMember);

            return il;
        }

        private static void EmitLoadValues(
            ILEmitter il,
            CollectionComparison member,
            LocalBuilder x,
            LocalBuilder y,
            LocalBuilder index)
        {
            var elementType = member.ElementType;
            var underlyingType = elementType.GetUnderlyingType();
            var comparisonType = elementType.GetComparisonType();

            if (comparisonType == ComparisonType.Hierarchicals)
            {
                il.LoadArgument(Arg.Context);
            }

            il.LoadLocal(x)
              .LoadLocal(index)
              .Call(member.GetItemMethod);
            if (comparisonType == ComparisonType.Comparables)
            {
                il.Store(underlyingType, LocalX, out var item)
                  .LoadAddress(item);
            }

            il.LoadLocal(y)
              .LoadLocal(index)
              .Call(member.GetItemMethod);

            if (comparisonType == ComparisonType.Hierarchicals)
            {
                il.LoadArgument(Arg.SetX)
                  .LoadArgument(Arg.SetY);
            }
        }

        private static void EmitLoadNullableValues(
            ILEmitter il,
            CollectionComparison member,
            LocalBuilder x,
            LocalBuilder y,
            LocalBuilder index,
            Label gotoNextMember)
        {
            var elementType = member.ElementType;
            var underlyingType = elementType.GetUnderlyingType();
            var comparisonType = elementType.GetComparisonType();

            il.LoadLocal(x)
              .LoadLocal(index)
              .Call(member.GetItemMethod)
              .Store(elementType, LocalX, out var nullableX);
            il.LoadLocal(y)
              .LoadLocal(index)
              .Call(member.GetItemMethod)
              .Store(elementType, LocalY, out var nullableY);

            il.CheckNullableValuesForNull(nullableX, nullableY, elementType, gotoNextMember);

            if (comparisonType == ComparisonType.Hierarchicals)
            {
                il.LoadArgument(Arg.Context);
            }

            var getValueMethod = elementType.GetPropertyGetter(MethodName.Value);
            il.LoadAddress(nullableX).Call(getValueMethod);
            if (comparisonType == ComparisonType.Comparables)
            {
                il.Store(underlyingType, out var address).LoadAddress(address);
            }

            il.LoadAddress(nullableY).Call(getValueMethod);

            if (comparisonType == ComparisonType.Hierarchicals)
            {
                il.LoadArgument(Arg.SetX)
                  .LoadArgument(Arg.SetY);
            }
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
              .Branch(OpCodes.Br_S, gotoNextMember)
              .MarkLabel(returnM1)
              .Return(-LocalX)
              .MarkLabel(checkIsDoneY)
              .LoadLocal(isDoneY)
              .Branch(OpCodes.Brfalse_S, out var loadValues)
              .Return(LocalX)
              .MarkLabel(loadValues);
        }

        private static (LocalBuilder countX, LocalBuilder countY) EmitLoadCounts(
            ILEmitter il,
            CollectionComparison comparison,
            LocalBuilder x,
            LocalBuilder y)
        {
            il.LoadLocal(x)
              .Call(comparison.GetLengthMethod)
              .Store(typeof(int), LocalCountX, out var countX)
              .LoadLocal(y)
              .Call(comparison.GetLengthMethod)
              .Store(typeof(int), LocalCountY, out var countY);

            EmitCheckForNegativeCount(il, countX, countY, comparison.Variable.VariableType);

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

        private static void EmitCheckMemberReferenceComparison(
            ILEmitter il,
            LocalBuilder x,
            LocalBuilder y,
            Label gotoNextMember)
        {
            il.LoadLocal(x)
              .LoadLocal(y)
              .Branch(OpCodes.Bne_Un_S, out var checkX)
              .Branch(OpCodes.Br, gotoNextMember)
              .MarkLabel(checkX)
              .LoadLocal(x)
              .Branch(OpCodes.Brtrue_S, out var checkY)
              .Return(-LocalX)
              .MarkLabel(checkY)
              .LoadLocal(y)
              .Branch(OpCodes.Brtrue_S, out var next)
              .Return(LocalX)
              .MarkLabel(next);
        }
    }
}
