using System;
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
    internal sealed class CollectionComparer
    {
        private static readonly MethodInfo ToArrayMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToArray));
        private static readonly MethodInfo GetComparerMethod = typeof(IComparerProvider).GetMethod(nameof(IComparerProvider.GetComparer));

        private readonly IResolver _resolver;
        private readonly IConfigurationProvider _configuration;
        private readonly EmitReferenceComparisonDelegate _emitReferenceComparison;
        private readonly EmitCheckIfLoopsAreDoneDelegate _emitCheckIfLoopsAreDone;

        public CollectionComparer(
            IResolver resolver,
            IConfigurationProvider configuration,
            EmitCheckIfLoopsAreDoneDelegate emitCheckIfLoopsAreDone,
            EmitReferenceComparisonDelegate emitReferenceComparison)
        {
            _resolver = resolver;
            _configuration = configuration;
            _emitCheckIfLoopsAreDone = emitCheckIfLoopsAreDone;
            _emitReferenceComparison = emitReferenceComparison;
        }

        public (LocalBuilder collectionX, LocalBuilder collectionY) EmitLoad(IVariable variable, ILEmitter il, Label gotoNext)
        {
            variable.Load(il, Arg.X).Store(variable.VariableType, out var collectionX);
            variable.Load(il, Arg.Y).Store(variable.VariableType, out var collectionY);

            _emitReferenceComparison(il, LoadLocal(collectionX), LoadLocal(collectionY), GoTo(gotoNext));

            return (collectionX, collectionY);
        }

        public void EmitArraySorting(ILEmitter il, Type elementType, LocalBuilder xArray, LocalBuilder yArray)
        {
            // todo: 2. compare default sorting and sorting with generated comparer - TrySZSort can work faster
            var useSimpleSorting = !_configuration.HasCustomComparer(elementType) && elementType.GetUnderlyingType().ImplementsGeneric(typeof(IComparable<>));

            if (useSimpleSorting) {
                EmitSortArray(il, elementType, xArray);
                EmitSortArray(il, elementType, yArray);
            } else {
                var getComparerMethod = GetComparerMethod.MakeGenericMethod(elementType);

                il.LoadArgument(Arg.Context)
                  .Call(getComparerMethod)
                  .Store(getComparerMethod.ReturnType, out var comparer);

                EmitSortArray(il, elementType, xArray, comparer);
                EmitSortArray(il, elementType, yArray, comparer);
            }
        }

        public ILEmitter CompareArrays(
            Type arrayType,
            Type ownerType,
            LocalBuilder xArray,
            LocalBuilder yArray,
            LocalBuilder countX,
            LocalBuilder countY,
            ILEmitter il,
            Label afterLoop)
        {
            il.LoadInteger(0)
              .Store(typeof(int), out var index)
              .DefineLabel(out var loopStart)
              .DefineLabel(out var continueLoop)
              .MarkLabel(loopStart);

            using (il.LocalsScope()) {
                il.AreSame(LoadLocal(index), LoadLocal(countX), out var isDoneX)
                  .AreSame(LoadLocal(index), LoadLocal(countY), out var isDoneY);
                _emitCheckIfLoopsAreDone(il, isDoneX, isDoneY, afterLoop);
            }

            using (il.LocalsScope()) {
                var itemVariable = new ArrayItemVariable(arrayType, ownerType, xArray, yArray, index);
                var itemComparison = _resolver.GetComparisonEmitter(itemVariable);

                return il
                    .Execute(
                        itemComparison.Emit(continueLoop),
                        itemComparison.EmitCheckForIntermediateResult(continueLoop))
                    .MarkLabel(continueLoop)
                    .Add(LoadLocal(index), LoadInteger(1))
                    .Store(index)
                    .GoTo(loopStart);
            }
        }

        public (LocalBuilder countX, LocalBuilder countY) EmitLoadCounts(Type arrayType, LocalBuilder arrayX, LocalBuilder arrayY, ILEmitter il)
        {
            il.EmitArrayLength(arrayType, arrayX, out var countX)
              .EmitArrayLength(arrayType, arrayY, out var countY);

            return (countX, countY);
        }

        private static void EmitSortArray(ILEmitter il, Type elementType, LocalBuilder array, LocalBuilder comparer)
        {
            var copyMethod = ToArrayMethod.MakeGenericMethod(elementType);
            var sortMethod = GetArraySortWithComparer(elementType);

            il.LoadLocal(array)
              .Call(copyMethod)
              .Store(array)
              .LoadLocal(array)
              .LoadLocal(comparer)
              .Call(sortMethod);
        }

        private static void EmitSortArray(ILEmitter il, Type elementType, LocalBuilder array)
        {
            var copyMethod = ToArrayMethod.MakeGenericMethod(elementType);
            var sortMethod = GetArraySortMethod(elementType);

            il.LoadLocal(array)
              .Call(copyMethod)
              .Store(array)
              .LoadLocal(array)
              .Call(sortMethod);
        }

        private static MethodInfo GetArraySortWithComparer(Type elementType) => typeof(Array)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Where(x => x.Name == nameof(Array.Sort) && x.IsGenericMethodDefinition)
            .Single(x => {
                var parameters = x.GetParameters();

                return parameters.Length == 2
                    && parameters[0].ParameterType.IsArray
                    && parameters[1].ParameterType.IsGenericType
                    && parameters[1].ParameterType.GetGenericTypeDefinition() == typeof(IComparer<>);
            })
            .MakeGenericMethod(elementType);

        private static MethodInfo GetArraySortMethod(Type elementType) => typeof(Array)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Where(x => x.Name == nameof(Array.Sort) && x.IsGenericMethodDefinition)
            .Single(x => {
                var parameters = x.GetParameters();
                return parameters.Length == 1 && parameters[0].ParameterType.IsArray;
            })
            .MakeGenericMethod(elementType);
    }
}
