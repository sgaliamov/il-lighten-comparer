using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Comparisons;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Emitters.Visitors
{
    internal sealed class EnumerableVisitor : CollectionVisitor
    {
        private const int LocalDoneX = 3;
        private const int LocalDoneY = 4;

        private readonly ComparerContext _context;
        private readonly Converter _converter;

        public EnumerableVisitor(
            ComparerContext context,
            StackVisitor stackVisitor,
            CompareVisitor compareVisitor,
            VariableLoader loader,
            Converter converter)
            : base(stackVisitor, compareVisitor, loader, converter)
        {
            _context = context;
            _converter = converter;
        }

        public ILEmitter Visit(EnumerableComparison comparison, ILEmitter il)
        {
            var (x, y, gotoNext) = EmitLoad(il, comparison);

            if (_context.GetConfiguration(comparison.Variable.OwnerType).IgnoreCollectionOrder)
            {
                EmitArraySorting(il, comparison.ElementType, x, y);
            }

            var (xEnumerator, yEnumerator) = EmitLoadEnumerators(il, comparison, x, y);

            // todo: think how to use try/finally block
            // the problem now with the inner `return` statements, it has to be `leave` instruction
            //il.BeginExceptionBlock(); 

            Loop(il, comparison, xEnumerator, yEnumerator, gotoNext);

            //il.BeginFinallyBlock();
            EmitDisposeEnumerators(il, xEnumerator, yEnumerator, gotoNext);

            //il.EndExceptionBlock();

            return il.MarkLabel(gotoNext);
        }

        private static (LocalBuilder xEnumerator, LocalBuilder yEnumerator) EmitLoadEnumerators(
            ILEmitter il,
            EnumerableComparison comparison,
            LocalBuilder xEnumerable,
            LocalBuilder yEnumerable)
        {
            il.LoadLocal(xEnumerable)
              .Call(comparison.GetEnumeratorMethod)
              .Store(comparison.EnumeratorType, LocalX, out var xEnumerator)
              .LoadLocal(yEnumerable)
              .Call(comparison.GetEnumeratorMethod)
              .Store(comparison.EnumeratorType, LocalY, out var yEnumerator);

            return (xEnumerator, yEnumerator);
        }

        private void Loop(
            ILEmitter il,
            EnumerableComparison comparison,
            LocalBuilder xEnumerator,
            LocalBuilder yEnumerator,
            Label gotoNextMember)
        {
            il.DefineLabel(out var continueLoop)
              .MarkLabel(continueLoop);

            var (xDone, yDone) = EmitMoveNext(il, xEnumerator, yEnumerator);

            EmitCheckIfLoopsAreDone(il, xDone, yDone, gotoNextMember);

            var elementType = comparison.ElementType;
            if (elementType.IsNullable())
            {
                var itemVariable = new EnumerableItemVariable(comparison.Variable.OwnerType, xEnumerator, yEnumerator);

                VisitNullable(il, itemVariable, continueLoop);
            }
            else
            {
                var itemComparison = _converter.CreateEnumerableItemComparison(
                    comparison.Variable.OwnerType,
                    xEnumerator,
                    yEnumerator);

                Visit(il, itemComparison, continueLoop);
            }
        }

        private static void EmitCheckIfLoopsAreDone(
            ILEmitter il,
            LocalBuilder xDone,
            LocalBuilder yDone,
            Label gotoNextMember)
        {
            il.LoadLocal(xDone)
              .Branch(OpCodes.Brfalse_S, out var checkY)
              .LoadLocal(yDone)
              .Branch(OpCodes.Brfalse_S, out var returnM1)
              .Branch(OpCodes.Br, gotoNextMember)
              .MarkLabel(returnM1)
              .Return(-1)
              .MarkLabel(checkY)
              .LoadLocal(yDone)
              .Branch(OpCodes.Brfalse_S, out var compare)
              .Return(1)
              .MarkLabel(compare);
        }

        private static (LocalBuilder xDone, LocalBuilder yDone) EmitMoveNext(
            ILEmitter il,
            LocalBuilder xEnumerator,
            LocalBuilder yEnumerator)
        {
            il.LoadLocal(xEnumerator)
              .Call(Method.MoveNext)
              .LoadConstant(0)
              .Emit(OpCodes.Ceq)
              .Store(typeof(int), LocalDoneX, out var xDone)
              .LoadLocal(yEnumerator)
              .Call(Method.MoveNext)
              .LoadConstant(0)
              .Emit(OpCodes.Ceq)
              .Store(typeof(int), LocalDoneY, out var yDone);

            return (xDone, yDone);
        }

        private static void EmitDisposeEnumerators(
            ILEmitter il,
            LocalBuilder xEnumerator,
            LocalBuilder yEnumerator,
            Label gotoNextMember)
        {
            il.LoadLocal(xEnumerator)
              .Branch(OpCodes.Brfalse_S, out var check)
              .LoadLocal(xEnumerator)
              .Call(Method.Dispose)
              .MarkLabel(check)
              .LoadLocal(yEnumerator)
              .Branch(OpCodes.Brfalse_S, gotoNextMember)
              .LoadLocal(yEnumerator)
              .Call(Method.Dispose);
        }
    }
}
