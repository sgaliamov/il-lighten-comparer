using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Config;
using ILLightenComparer.Shared.Comparisons;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Comparer.Comparisons
{
    internal sealed class EnumerablesComparison : IComparisonEmitter
    {
        private static readonly MethodInfo MoveNextMethod = typeof(IEnumerator).GetMethod(nameof(IEnumerator.MoveNext), Type.EmptyTypes);
        private static readonly MethodInfo DisposeMethod = typeof(IDisposable).GetMethod(nameof(IDisposable.Dispose), Type.EmptyTypes);

        private readonly Type _elementType;
        private readonly Type _enumeratorType;
        private readonly MethodInfo _getEnumeratorMethod;
        private readonly IVariable _variable;
        private readonly ArrayComparer _arrayComparer;
        private readonly CollectionComparer _collectionComparer;
        private readonly EmitCheckIfLoopsAreDoneDelegate _emitCheckIfLoopsAreDone;
        private readonly IResolver _resolver;
        private readonly IConfigurationProvider _configuration;

        private EnumerablesComparison(
            IResolver resolver,
            EmitCheckIfLoopsAreDoneDelegate emitCheckIfLoopsAreDone,
            IConfigurationProvider configuration,
            IVariable variable)
        {
            _emitCheckIfLoopsAreDone = emitCheckIfLoopsAreDone;
            _resolver = resolver;
            _configuration = configuration;
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
                .GetMethod(nameof(IEnumerable.GetEnumerator), Type.EmptyTypes);

            _arrayComparer = new ArrayComparer(resolver, CustomEmitters.EmitCheckIfArrayLoopsAreDone);
            _collectionComparer = new CollectionComparer(configuration, CustomEmitters.EmitReferenceComparison);
        }

        public static EnumerablesComparison Create(
            IResolver comparisons,
            EmitCheckIfLoopsAreDoneDelegate emitCheckIfLoopsAreDone,
            IConfigurationProvider configuration,
            IVariable variable)
        {
            var variableType = variable.VariableType;
            if (variableType.ImplementsGeneric(typeof(IEnumerable<>)) && !variableType.IsArray) {
                return new EnumerablesComparison(comparisons, emitCheckIfLoopsAreDone, configuration, variable);
            }

            return null;
        }

        public ILEmitter Emit(ILEmitter il, Label gotoNext)
        {
            var (x, y) = _collectionComparer.EmitLoad(_variable, il, gotoNext);

            if (_configuration.Get(_variable.OwnerType).IgnoreCollectionOrder) {
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

        public ILEmitter EmitCheckForIntermediateResult(ILEmitter il, Label _) => il;

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

        private void Loop(LocalBuilder xEnumerator, LocalBuilder yEnumerator, ILEmitter il, Label gotoNext)
        {
            il.DefineLabel(out var continueLoop).MarkLabel(continueLoop);

            using (il.LocalsScope()) {
                var (xDone, yDone) = EmitMoveNext(xEnumerator, yEnumerator, il);

                _emitCheckIfLoopsAreDone(il, xDone, yDone, gotoNext);
            }

            using (il.LocalsScope()) {
                var itemVariable = new EnumerableItemVariable(_variable.OwnerType, xEnumerator, yEnumerator);

                var itemComparison = _resolver.GetComparisonEmitter(itemVariable);
                itemComparison.Emit(il, continueLoop);
                itemComparison.EmitCheckForIntermediateResult(il, continueLoop);
            }
        }

        private static (LocalBuilder xDone, LocalBuilder yDone) EmitMoveNext(
            LocalBuilder xEnumerator,
            LocalBuilder yEnumerator,
            ILEmitter il)
        {
            il.AreSame(il => il.LoadLocal(xEnumerator).Call(MoveNextMethod),
                       il => il.LoadInteger(0),
                       out var xDone)
              .AreSame(il => il.LoadLocal(yEnumerator).Call(MoveNextMethod),
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
              .IfFalse_S(out var check)
              .LoadLocal(xEnumerator)
              .Call(DisposeMethod)
              .MarkLabel(check)
              .LoadLocal(yEnumerator)
              .IfFalse(gotoNext)
              .LoadLocal(yEnumerator)
              .Call(DisposeMethod);
        }
    }
}
