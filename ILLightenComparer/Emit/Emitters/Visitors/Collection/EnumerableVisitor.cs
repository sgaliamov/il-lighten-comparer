using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Comparisons;
using ILLightenComparer.Emit.Reflection;
using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit.Emitters.Visitors.Collection
{
    internal sealed class EnumerableVisitor : CollectionVisitor
    {
        private const int DoneX = 3;
        private const int DoneY = 4;

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
            Label gotoNext)
        {
            il.DefineLabel(out var continueLoop)
              .MarkLabel(continueLoop);

            var (xDone, yDone) = EmitMoveNext(il, xEnumerator, yEnumerator);

            EmitCheckIfLoopsAreDone(il, xDone, yDone, gotoNext);

            var elementType = comparison.ElementType;
            var itemComparison = _converter.CreateEnumerableItemComparison(
                comparison.Variable.OwnerType,
                xEnumerator,
                yEnumerator);

            Visit(il, itemComparison, elementType, continueLoop);
        }

        private static void EmitCheckIfLoopsAreDone(
            ILEmitter il,
            LocalBuilder xDone,
            LocalBuilder yDone,
            Label gotoNext)
        {
            il.LoadLocal(xDone)
              .Branch(OpCodes.Brfalse_S, out var checkY)
              .LoadLocal(yDone)
              .Branch(OpCodes.Brfalse_S, out var returnM1)
              .Branch(OpCodes.Br, gotoNext)
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
              .Store(typeof(int), DoneX, out var xDone)
              .LoadLocal(yEnumerator)
              .Call(Method.MoveNext)
              .LoadConstant(0)
              .Emit(OpCodes.Ceq)
              .Store(typeof(int), DoneY, out var yDone);

            return (xDone, yDone);
        }

        private static void EmitDisposeEnumerators(
            ILEmitter il,
            LocalBuilder xEnumerator,
            LocalBuilder yEnumerator,
            Label gotoNext)
        {
            il.LoadLocal(xEnumerator)
              .Branch(OpCodes.Brfalse_S, out var check)
              .LoadLocal(xEnumerator)
              .Call(Method.Dispose)
              .MarkLabel(check)
              .LoadLocal(yEnumerator)
              .Branch(OpCodes.Brfalse_S, gotoNext)
              .LoadLocal(yEnumerator)
              .Call(Method.Dispose);
        }
    }
}
