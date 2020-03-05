using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Config;
using ILLightenComparer.Emitters.Comparisons.Collection;
using ILLightenComparer.Emitters.Variables;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Emitters.Comparisons
{
    internal sealed class EnumerablesComparison : IComparison
    {
        public Type ElementType { get; }
        public Type EnumeratorType { get; }
        public MethodInfo GetEnumeratorMethod { get; }

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
            Variable = variable ?? throw new ArgumentNullException(nameof(variable));

            ElementType = variable
                          .VariableType
                          .FindGenericInterface(typeof(IEnumerable<>))
                          .GetGenericArguments()
                          .SingleOrDefault()
                          ?? throw new ArgumentException(nameof(variable));

            // todo: use read enumerator, not virtual
            EnumeratorType = typeof(IEnumerator<>).MakeGenericType(ElementType);

            GetEnumeratorMethod = typeof(IEnumerable<>)
                                  .MakeGenericType(ElementType)
                                  .GetMethod(MethodName.GetEnumerator, Type.EmptyTypes);

            _arrayComparer = new ArrayComparer(comparisons);
            _collectionComparer = new CollectionComparer(configurations);
        }

        public IVariable Variable { get; }
        public bool PutsResultInStack => false;

        public ILEmitter Accept(ILEmitter il, Label afterLoop)
        {
            var (x, y) = _collectionComparer.EmitLoad(this, il, afterLoop);

            if (_configurations.Get(Variable.OwnerType).IgnoreCollectionOrder) {
                return EmitCompareAsSortedArrays(il, afterLoop, x, y);
            }

            var (xEnumerator, yEnumerator) = EmitLoadEnumerators(x, y, il);

            // todo: think how to use try/finally block
            // the problem now with the inner `return` statements, it has to be `leave` instruction
            //il.BeginExceptionBlock(); 

            Loop(xEnumerator, yEnumerator, il, afterLoop);

            //il.BeginFinallyBlock();
            EmitDisposeEnumerators(xEnumerator, yEnumerator, il, afterLoop);

            //il.EndExceptionBlock();

            return il;
        }

        private ILEmitter EmitCompareAsSortedArrays(
            ILEmitter il,
            Label gotoNext,
            LocalBuilder x,
            LocalBuilder y)
        {
            _collectionComparer.EmitArraySorting(il, ElementType, x, y);

            var arrayType = ElementType.MakeArrayType();

            var (countX, countY) = _arrayComparer.EmitLoadCounts(arrayType, x, y, il);

            return _arrayComparer.Compare(arrayType, Variable.OwnerType, x, y, countX, countY, il, gotoNext);
        }

        private (LocalBuilder xEnumerator, LocalBuilder yEnumerator) EmitLoadEnumerators(
            LocalBuilder xEnumerable,
            LocalBuilder yEnumerable,
            ILEmitter il)
        {
            il.LoadLocal(xEnumerable)
              .Call(GetEnumeratorMethod)
              .Store(EnumeratorType, out var xEnumerator)
              .LoadLocal(yEnumerable)
              .Call(GetEnumeratorMethod)
              .Store(EnumeratorType, out var yEnumerator);

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
                var itemVariable = new EnumerableItemVariable(Variable.OwnerType, xEnumerator, yEnumerator);

                var itemComparison = _comparisons.GetComparison(itemVariable);
                itemComparison.Accept(il, continueLoop);

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

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

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
    }
}
