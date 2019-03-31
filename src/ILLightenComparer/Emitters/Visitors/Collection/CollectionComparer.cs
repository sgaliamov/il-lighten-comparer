using System;
using System.Reflection.Emit;
using ILLightenComparer.Config;
using ILLightenComparer.Emitters.Comparisons;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using ILLightenComparer.Shared;

namespace ILLightenComparer.Emitters.Visitors.Collection
{
    internal sealed class CollectionComparer
    {
        private readonly IConfigurationProvider _configurations;
        private readonly VariableLoader _loader;

        public CollectionComparer(IConfigurationProvider configurations, VariableLoader loader)
        {
            _configurations = configurations;
            _loader = loader;
        }

        public (LocalBuilder collectionX, LocalBuilder collectionY) EmitLoad(IComparison comparison, ILEmitter il, Label gotoNext)
        {
            var variable = comparison.Variable;
            variable.Load(_loader, il, Arg.X).Store(variable.VariableType, out var collectionX);
            variable.Load(_loader, il, Arg.Y).Store(variable.VariableType, out var collectionY);

            il.EmitReferenceComparison(collectionX, collectionY, gotoNext);

            return (collectionX, collectionY);
        }

        public void EmitArraySorting(ILEmitter il, Type elementType, LocalBuilder xArray, LocalBuilder yArray)
        {
            // todo: compare default sorting and sorting with generated comparer - TrySZSort can work faster
            var useSimpleSorting = !_configurations.HasCustomComparer(elementType)
                                   && elementType.GetUnderlyingType().ImplementsGeneric(typeof(IComparable<>));
            if (useSimpleSorting)
            {
                EmitSortArray(il, elementType, xArray);
                EmitSortArray(il, elementType, yArray);
            }
            else
            {
                var getComparerMethod = Method.GetComparer.MakeGenericMethod(elementType);

                il.LoadArgument(Arg.Context)
                  .Emit(OpCodes.Call, getComparerMethod)
                  .Store(getComparerMethod.ReturnType, out var comparer);

                EmitSortArray(il, elementType, xArray, comparer);
                EmitSortArray(il, elementType, yArray, comparer);
            }
        }

        private static void EmitSortArray(ILEmitter il, Type elementType, LocalBuilder array, LocalBuilder comparer)
        {
            var copyMethod = Method.ToArray.MakeGenericMethod(elementType);
            var sortMethod = Method.GetArraySortWithComparer(elementType);

            il.LoadLocal(array)
              .Call(copyMethod)
              .Store(array)
              .LoadLocal(array)
              .LoadLocal(comparer)
              .Call(sortMethod);
        }

        private static void EmitSortArray(ILEmitter il, Type elementType, LocalBuilder array)
        {
            var copyMethod = Method.ToArray.MakeGenericMethod(elementType);
            var sortMethod = Method.GetArraySort(elementType);

            il.LoadLocal(array)
              .Call(copyMethod)
              .Store(array)
              .LoadLocal(array)
              .Call(sortMethod);
        }
    }
}
