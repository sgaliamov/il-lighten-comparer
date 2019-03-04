﻿using System.Reflection.Emit;
using ILLightenComparer.Config;
using ILLightenComparer.Emitters.Comparisons;
using ILLightenComparer.Emitters.Variables;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using ILLightenComparer.Shared;

namespace ILLightenComparer.Emitters.Visitors.Collection
{
    internal sealed class EnumerableVisitor : CollectionVisitor
    {
        private readonly ArrayComparer _arrayComparer;

        private readonly CompareVisitor _compareVisitor;
        private readonly IConfigurationProvider _configuration;
        private readonly Converter _converter;

        public EnumerableVisitor(
            IConfigurationProvider configuration,
            CompareVisitor compareVisitor,
            VariableLoader loader,
            Converter converter)
            : base(loader)
        {
            _configuration = configuration;
            _compareVisitor = compareVisitor;
            _converter = converter;
            _arrayComparer = new ArrayComparer(compareVisitor, converter);
        }

        public ILEmitter Visit(EnumerablesComparison comparison, ILEmitter il, Label afterLoop)
        {
            var (x, y) = EmitLoad(comparison, il, afterLoop);

            if (_configuration.Get(comparison.Variable.OwnerType).IgnoreCollectionOrder)
            {
                return EmitCompareAsSortedArrays(comparison, il, afterLoop, x, y);
            }

            var (xEnumerator, yEnumerator) = EmitLoadEnumerators(comparison, x, y, il);

            // todo: think how to use try/finally block
            // the problem now with the inner `return` statements, it has to be `leave` instruction
            //il.BeginExceptionBlock(); 

            Loop(comparison, xEnumerator, yEnumerator, il, afterLoop);

            //il.BeginFinallyBlock();
            EmitDisposeEnumerators(xEnumerator, yEnumerator, il, afterLoop);

            //il.EndExceptionBlock();

            return il;
        }

        private ILEmitter EmitCompareAsSortedArrays(EnumerablesComparison comparison, ILEmitter il, Label gotoNext, LocalBuilder x, LocalBuilder y)
        {
            EmitArraySorting(il, comparison.ElementType, x, y);

            var arrayType = comparison.ElementType.MakeArrayType();

            var (countX, countY) = _arrayComparer.EmitLoadCounts(arrayType, x, y, il);

            return _arrayComparer.Compare(arrayType, comparison.Variable.OwnerType, x, y, countX, countY, il, gotoNext);
        }

        private static (LocalBuilder xEnumerator, LocalBuilder yEnumerator) EmitLoadEnumerators(
            EnumerablesComparison comparison,
            LocalBuilder xEnumerable,
            LocalBuilder yEnumerable,
            ILEmitter il)
        {
            il.LoadLocal(xEnumerable)
              .Call(comparison.GetEnumeratorMethod)
              .Store(comparison.EnumeratorType, out var xEnumerator)
              .LoadLocal(yEnumerable)
              .Call(comparison.GetEnumeratorMethod)
              .Store(comparison.EnumeratorType, out var yEnumerator);

            return (xEnumerator, yEnumerator);
        }

        private void Loop(
            IComparison comparison,
            LocalBuilder xEnumerator,
            LocalBuilder yEnumerator,
            ILEmitter il,
            Label gotoNext)
        {
            il.DefineLabel(out var continueLoop).MarkLabel(continueLoop);

            using (il.LocalsScope())
            {
                var (xDone, yDone) = EmitMoveNext(xEnumerator, yEnumerator, il);

                EmitCheckIfLoopsAreDone(xDone, yDone, il, gotoNext);
            }

            using (il.LocalsScope())
            {
                var itemVariable = new EnumerableItemVariable(comparison.Variable.OwnerType, xEnumerator, yEnumerator);

                var itemComparison = _converter.CreateComparison(itemVariable);
                itemComparison.Accept(_compareVisitor, il, continueLoop);
            
                if (itemComparison.ResultInStack)
                {
                    il.EmitReturnNotZero(continueLoop);
                }
            }
        }

        private static void EmitCheckIfLoopsAreDone(
            LocalBuilder xDone,
            LocalBuilder yDone,
            ILEmitter il,
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
            LocalBuilder xEnumerator,
            LocalBuilder yEnumerator,
            ILEmitter il)
        {
            il.LoadLocal(xEnumerator)
              .Call(Method.MoveNext)
              .LoadConstant(0)
              .Emit(OpCodes.Ceq)
              .Store(typeof(int), out var xDone)
              .LoadLocal(yEnumerator)
              .Call(Method.MoveNext)
              .LoadConstant(0)
              .Emit(OpCodes.Ceq)
              .Store(typeof(int), out var yDone);

            return (xDone, yDone);
        }

        private static void EmitDisposeEnumerators(
            LocalBuilder xEnumerator,
            LocalBuilder yEnumerator,
            ILEmitter il,
            Label gotoNext)
        {
            il.LoadLocal(xEnumerator)
              .Branch(OpCodes.Brfalse_S, out var check)
              .LoadLocal(xEnumerator)
              .Call(Method.Dispose)
              .MarkLabel(check)
              .LoadLocal(yEnumerator)
              .Branch(OpCodes.Brfalse, gotoNext)
              .LoadLocal(yEnumerator)
              .Call(Method.Dispose);
        }
    }
}