using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Config;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Comparer.Comparisons
{
    internal sealed class CollectionComparer
    {
        private static readonly MethodInfo ToArrayMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToArray));
        private static readonly MethodInfo GetComparerMethod = typeof(IComparerProvider).GetMethod(nameof(IComparerProvider.GetComparer));
        private readonly IConfigurationProvider _configuration;
        private readonly EmitReferenceComparisonDelegate _emitReferenceComparison;

        public CollectionComparer(IConfigurationProvider configuration, EmitReferenceComparisonDelegate emitReferenceComparison)
        {
            _configuration = configuration;
            _emitReferenceComparison = emitReferenceComparison;
        }

        public (LocalBuilder collectionX, LocalBuilder collectionY) EmitLoad(IVariable variable, ILEmitter il, Label gotoNext)
        {
            variable.Load(il, Arg.X).Store(variable.VariableType, out var collectionX);
            variable.Load(il, Arg.Y).Store(variable.VariableType, out var collectionY);

            _emitReferenceComparison(il, collectionX, collectionY, gotoNext);

            return (collectionX, collectionY);
        }

        public void EmitArraySorting(ILEmitter il, Type elementType, LocalBuilder xArray, LocalBuilder yArray)
        {
            // todo: 2. compare default sorting and sorting with generated comparer - TrySZSort can work faster
            var useSimpleSorting = !_configuration.HasCustomComparer(elementType)
                                   && elementType.GetUnderlyingType().ImplementsGeneric(typeof(IComparable<>));
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
