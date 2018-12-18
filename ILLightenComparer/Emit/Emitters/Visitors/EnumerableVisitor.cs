using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Comparisons;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Emitters.Visitors
{
    internal sealed class EnumerableVisitor
    {
        private const int LocalX = Arg.X; // 1
        private const int LocalY = Arg.Y; // 2
        private const int LocalDoneX = 3;
        private const int LocalDoneY = 4;

        private readonly CompareVisitor _compareVisitor;
        private readonly Converter _converter;
        private readonly VariableLoader _loader;
        private readonly StackVisitor _stackVisitor;

        public EnumerableVisitor(
            StackVisitor stackVisitor,
            CompareVisitor compareVisitor,
            VariableLoader loader,
            Converter converter)
        {
            _compareVisitor = compareVisitor;
            _loader = loader;
            _converter = converter;
            _stackVisitor = stackVisitor;
        }

        public ILEmitter Visit(EnumerableComparison comparison, ILEmitter il)
        {
            var variable = comparison.Variable;
            il.DefineLabel(out var gotoNextMember)
              .DefineLabel(out var startLoop);

            variable.Load(_loader, il, Arg.X).Store(variable.VariableType, LocalX, out var xEnumerable);
            variable.Load(_loader, il, Arg.Y).Store(variable.VariableType, LocalY, out var yEnumerable);

            il.EmitCheckReferenceComparison(xEnumerable, yEnumerable, gotoNextMember);

            il.LoadLocal(xEnumerable)
              .Call(comparison.GetEnumeratorMethod)
              .Store(comparison.EnumeratorType, LocalX, out var xEnumerator)
              .LoadLocal(yEnumerable)
              .Call(comparison.GetEnumeratorMethod)
              .Store(comparison.EnumeratorType, LocalY, out var yEnumerator);

            using (il.TryBlock())
            {
                il.MarkLabel(startLoop);
                var (xDone, yDone) = EmitMoveNext(il, xEnumerator, yEnumerator);

                EmitIfLoopIsDone(il, xDone, yDone, gotoNextMember);

                var itemComparison = _converter.CreateEnumerableItemComparison(
                    variable.OwnerType,
                    xEnumerator, 
                    yEnumerator);

                itemComparison.LoadVariables(_stackVisitor, il, gotoNextMember);
                itemComparison.Accept(_compareVisitor, il)
                              .EmitReturnNotZero(startLoop);

                using (il.FinallyBlock())
                {
                    EmitDisposeEnumerators(il, xEnumerator, yEnumerator, gotoNextMember);
                }
            }

            return il.MarkLabel(gotoNextMember);
        }

        private static void EmitIfLoopIsDone(
            ILEmitter il,
            LocalBuilder xDone,
            LocalBuilder yDone,
            Label gotoNextMember)
        {
            il.LoadLocal(xDone)
              .Branch(OpCodes.Brfalse_S, out var isYDone)
              .LoadLocal(yDone)
              .Branch(OpCodes.Brfalse_S, out var returnM1)
              .Branch(OpCodes.Leave_S, gotoNextMember)
              .MarkLabel(returnM1)
              .Return(-1)
              .MarkLabel(isYDone)
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
              .Branch(OpCodes.Brfalse_S, out var yDispose)
              .LoadLocal(xEnumerator)
              .Call(Method.Dispose)
              .MarkLabel(yDispose)
              .LoadLocal(yEnumerator)
              .Branch(OpCodes.Brfalse_S, gotoNextMember)
              .LoadLocal(yEnumerator)
              .Call(Method.Dispose);
        }
    }
}
