using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Config;
using ILLightenComparer.Extensions;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;
using static Illuminator.Functional;

namespace ILLightenComparer.Shared.Comparisons
{
    internal sealed class EnumerablesComparison : IComparisonEmitter
    {
        private readonly Type _elementType;
        private readonly Type _enumeratorType;
        private readonly MethodInfo _moveNextMethod;
        private readonly MethodInfo _getCurrentMethod;
        private readonly MethodInfo _getEnumeratorMethod;
        private readonly ArrayComparisonEmitter _arrayComparisonEmitter;
        private readonly IVariable _variable;
        private readonly IConfigurationProvider _configuration;
        private readonly IResolver _resolver;
        private readonly EmitCheckIfLoopsAreDoneDelegate _emitCheckIfLoopsAreDone;
        private readonly int _defaultResult;

        private EnumerablesComparison(
            IResolver resolver,
            int defaultResult,
            ArrayComparisonEmitter arrayComparisonEmitter,
            EmitCheckIfLoopsAreDoneDelegate emitCheckIfLoopsAreDone,
            IConfigurationProvider configuration,
            IVariable variable)
        {
            _resolver = resolver;
            _defaultResult = defaultResult;
            _arrayComparisonEmitter = arrayComparisonEmitter;
            _emitCheckIfLoopsAreDone = emitCheckIfLoopsAreDone;
            _configuration = configuration;
            _variable = variable;

            var variableType = variable.VariableType;

            _elementType = variableType
                .FindGenericInterface(typeof(IEnumerable<>))
                .GetGenericArguments()
                .Single();

            _getEnumeratorMethod = variableType.FindMethod(nameof(IEnumerable.GetEnumerator), Type.EmptyTypes);
            _enumeratorType = _getEnumeratorMethod.ReturnType;
            _moveNextMethod = _enumeratorType.FindMethod(nameof(IEnumerator.MoveNext), Type.EmptyTypes);
            _getCurrentMethod = _enumeratorType.GetPropertyGetter(nameof(IEnumerator.Current));
        }

        public static EnumerablesComparison Create(
            IResolver comparisons,
            int defaultResult,
            ArrayComparisonEmitter arrayComparisonEmitter,
            EmitCheckIfLoopsAreDoneDelegate emitCheckIfLoopsAreDone,
            IConfigurationProvider configuration,
            IVariable variable)
        {
            var variableType = variable.VariableType;
            if (variableType.ImplementsGeneric(typeof(IEnumerable<>)) && !variableType.IsArray) {
                return new EnumerablesComparison(comparisons, defaultResult, arrayComparisonEmitter, emitCheckIfLoopsAreDone, configuration, variable);
            }

            return null;
        }

        public ILEmitter Emit(ILEmitter il, Label gotoNext)
        {
            var (x, y) = _arrayComparisonEmitter.EmitLoad(_variable, il, gotoNext);

            if (_configuration.Get(_variable.OwnerType).IgnoreCollectionOrder) {
                return EmitCompareAsSortedArrays(il, gotoNext, x, y);
            }

            var (xEnumerator, yEnumerator) = EmitLoadEnumerators(il, x, y);

            // todo: 1. think how to use try/finally block
            // the problem now with the inner `return` statements, it has to be `leave` instruction
            //il.BeginExceptionBlock(); 

            Loop(il, xEnumerator, yEnumerator, gotoNext);

            //il.BeginFinallyBlock();
            EmitDisposeEnumerators(il, xEnumerator, yEnumerator);

            //il.EndExceptionBlock();

            return il;
        }

        public ILEmitter Emit(ILEmitter il) => il
            .DefineLabel(out var exit)
            .Execute(this.Emit(exit))
            .MarkLabel(exit)
            .Return(_defaultResult);

        public ILEmitter EmitCheckForIntermediateResult(ILEmitter il, Label _) => il;

        private ILEmitter EmitCompareAsSortedArrays(ILEmitter il, Label gotoNext, LocalBuilder x, LocalBuilder y)
        {
            var hasCustomComparer = _configuration.HasCustomComparer(_elementType);

            il.EmitArraySorting(hasCustomComparer, _elementType, x, y);

            var arrayType = _elementType.MakeArrayType();

            return _arrayComparisonEmitter.EmitCompareArrays(il, arrayType, _variable.OwnerType, x, y, gotoNext);
        }

        private (LocalBuilder xEnumerator, LocalBuilder yEnumerator) EmitLoadEnumerators(ILEmitter il, LocalBuilder xEnumerable, LocalBuilder yEnumerable)
        {
            il.Call(_getEnumeratorMethod, LoadCaller(xEnumerable))
              .Store(_enumeratorType, out var xEnumerator)
              .Call(_getEnumeratorMethod, LoadCaller(yEnumerable))
              .Store(_enumeratorType, out var yEnumerator);

            // todo: 3. check enumerators for null?

            return (xEnumerator, yEnumerator);
        }

        private void Loop(ILEmitter il, LocalBuilder xEnumerator, LocalBuilder yEnumerator, Label gotoNext)
        {
            il.DefineLabel(out var loopStart);

            using (il.LocalsScope()) {
                il.MarkLabel(loopStart);
                var (xDone, yDone) = EmitMoveNext(xEnumerator, yEnumerator, il);

                _emitCheckIfLoopsAreDone(il, xDone, yDone, gotoNext);
            }

            using (il.LocalsScope()) {
                var enumerators = new Dictionary<ushort, LocalBuilder>(2) {
                    [Arg.X] = xEnumerator,
                    [Arg.Y] = yEnumerator
                };

                var itemVariable = new EnumerableItemVariable(_enumeratorType, _elementType, _getCurrentMethod, enumerators);
                var itemComparison = _resolver.GetComparisonEmitter(itemVariable);

                il.Execute(itemComparison.Emit(loopStart))
                  .Execute(itemComparison.EmitCheckForIntermediateResult(loopStart));
            }
        }

        private (LocalBuilder xDone, LocalBuilder yDone) EmitMoveNext(LocalBuilder xEnumerator, LocalBuilder yEnumerator, ILEmitter il)
        {
            // todo: 3. it's possible to use "not done" flag. it will simplify emitted code in _emitCheckIfLoopsAreDone.
            il.AreSame(Call(_moveNextMethod, LoadCaller(xEnumerator)), LoadInteger(0), out var xDone)
              .AreSame(Call(_moveNextMethod, LoadCaller(yEnumerator)), LoadInteger(0), out var yDone);

            return (xDone, yDone);
        }

        private static void EmitDisposeEnumerators(ILEmitter il, LocalBuilder xEnumerator, LocalBuilder yEnumerator) => il
            .LoadCaller(xEnumerator)
            .Call(Methods.DisposeMethod)
            .LoadCaller(yEnumerator)
            .Call(Methods.DisposeMethod);
    }
}
