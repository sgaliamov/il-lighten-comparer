using System;
using System.Reflection.Emit;
using ILLightenComparer.Config;
using ILLightenComparer.Emitters.Variables;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Emitters.Comparisons.Collection
{
    internal sealed class CollectionComparer
    {
        private readonly IConfigurationProvider _configurations;

        public CollectionComparer(IConfigurationProvider configurations) => _configurations = configurations;

        public (LocalBuilder collectionX, LocalBuilder collectionY) EmitLoad(IVariable variable, ILEmitter il, Label gotoNext)
        {
            variable.Load(il, Arg.X).Store(variable.VariableType, out var collectionX);
            variable.Load(il, Arg.Y).Store(variable.VariableType, out var collectionY);

            il.EmitReferenceComparison(collectionX, collectionY, gotoNext);

            return (collectionX, collectionY);
        }

        public void EmitArraySorting(ILEmitter il, Type elementType, LocalBuilder xArray, LocalBuilder yArray)
        {
            // todo: compare default sorting and sorting with generated comparer - TrySZSort can work faster
            var useSimpleSorting = !_configurations.HasCustomComparer(elementType)
                                   && elementType.GetUnderlyingType().ImplementsGeneric(typeof(IComparable<>));
            if (useSimpleSorting) {
                EmitSortArray(il, elementType, xArray);
                EmitSortArray(il, elementType, yArray);
            } else {
                var getComparerMethod = Method.GetComparer.MakeGenericMethod(elementType);

                il.LoadArgument(Arg.Context)
                  .Call(getComparerMethod)
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
