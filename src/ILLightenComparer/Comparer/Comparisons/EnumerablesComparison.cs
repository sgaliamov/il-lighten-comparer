using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Comparer.Comparisons.Collection;
using ILLightenComparer.Config;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using ILLightenComparer.Shared;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Comparer.Comparisons
{
    internal sealed class EnumerablesComparison : IStepEmitter
    {
        private readonly Type _elementType;
        private readonly Type _enumeratorType;
        private readonly MethodInfo _getEnumeratorMethod;
        private readonly IVariable _variable;
        private readonly ArrayComparer _arrayComparer;
        private readonly CollectionComparer _collectionComparer;
        private readonly ComparisonResolver _comparisons;
        private readonly IConfigurationProvider _configurations;

        private EnumerablesComparison(
            ComparisonResolver comparisons,
            IConfigurationProvider configurations,
            IVariable variable)
        {
            _comparisons = comparisons;
            _configurations = configurations;
            _variable = variable ?? throw new ArgumentNullException(nameof(variable));

            _elementType = variable
                          .VariableType
                          .FindGenericInterface(typeof(IEnumerable<>))
                          .GetGenericArguments()
                          .SingleOrDefault()
                          ?? throw new ArgumentException(nameof(variable));

            // todo: 2. use read enumerator, not virtual
            _enumeratorType = typeof(IEnumerator<>).MakeGenericType(_elementType);

            _getEnumeratorMethod = typeof(IEnumerable<>)
                                  .MakeGenericType(_elementType)
                                  .GetMethod(MethodName.GetEnumerator, Type.EmptyTypes);

            _arrayComparer = new ArrayComparer(comparisons);
            _collectionComparer = new CollectionComparer(configurations);
        }

        public static EnumerablesComparison Create(
            ComparisonResolver comparisons,
            IConfigurationProvider configurations,
            IVariable variable)
        {
            var variableType = variable.VariableType;
            if (variableType.ImplementsGeneric(typeof(IEnumerable<>)) && !variableType.IsArray) {
                return new EnumerablesComparison(comparisons, configurations, variable);
            }

            return null;
        }

        public bool PutsResultInStack => false;

        public ILEmitter Emit(ILEmitter il, Label gotoNext)
        {
            var (x, y) = _collectionComparer.EmitLoad(_variable, il, gotoNext);

            if (_configurations.Get(_variable.OwnerType).IgnoreCollectionOrder) {
                return EmitCompareAsSortedArrays(il, gotoNext, x, y);
            }

            var (xEnumerator, yEnumerator) = EmitLoadEnumerators(x, y, il);

            // todo: 1. think how to use try/finally block
            // the problem now with the inner `return` statements, it has to be `leave` instruction
            //il.BeginExceptionBlock(); 

            Loop(xEnumerator, yEnumerator, il, gotoNext);

            //il.BeginFinallyBlock();
            EmitDisposeEnumerators(xEnumerator, yEnumerator, il, gotoNext);

            //il.EndExceptionBlock();

            return il;
        }

        public ILEmitter Emit(ILEmitter il)
        {
            il.DefineLabel(out var exit);

            return Emit(il, exit)
                .MarkLabel(exit)
                .Return(0);
        }

        private ILEmitter EmitCompareAsSortedArrays(
            ILEmitter il,
            Label gotoNext,
            LocalBuilder x,
            LocalBuilder y)
        {
            _collectionComparer.EmitArraySorting(il, _elementType, x, y);

            var arrayType = _elementType.MakeArrayType();

            var (countX, countY) = _arrayComparer.EmitLoadCounts(arrayType, x, y, il);

            return _arrayComparer.Compare(arrayType, _variable.OwnerType, x, y, countX, countY, il, gotoNext);
        }

        private (LocalBuilder xEnumerator, LocalBuilder yEnumerator) EmitLoadEnumerators(
            LocalBuilder xEnumerable,
            LocalBuilder yEnumerable,
            ILEmitter il)
        {
            il.LoadLocal(xEnumerable)
              .Call(_getEnumeratorMethod)
              .Store(_enumeratorType, out var xEnumerator)
              .LoadLocal(yEnumerable)
              .Call(_getEnumeratorMethod)
              .Store(_enumeratorType, out var yEnumerator);

            return (xEnumerator, yEnumerator);
        }

        private void Loop(
            LocalBuilder xEnumerator,
            LocalBuilder yEnumerator,
            ILEmitter il,
            Label gotoNext)
        {
            il.DefineLabel(out var continueLoop).MarkLabel(continueLoop);

            using (il.LocalsScope()) {
                var (xDone, yDone) = EmitMoveNext(xEnumerator, yEnumerator, il);

                EmitCheckIfLoopsAreDone(xDone, yDone, il, gotoNext);
            }

            using (il.LocalsScope()) {
                var itemVariable = new EnumerableItemVariable(_variable.OwnerType, xEnumerator, yEnumerator);

                var itemComparison = _comparisons.GetComparison(itemVariable);
                itemComparison.Emit(il, continueLoop);

                if (itemComparison.PutsResultInStack) {
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
              .GoTo(gotoNext)
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
            il.AreSame(il => il.LoadLocal(xEnumerator).Call(Method.MoveNext),
                       il => il.LoadInteger(0),
                       out var xDone)
              .AreSame(il => il.LoadLocal(yEnumerator).Call(Method.MoveNext),
                       il => il.LoadInteger(0),
                       out var yDone);

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
