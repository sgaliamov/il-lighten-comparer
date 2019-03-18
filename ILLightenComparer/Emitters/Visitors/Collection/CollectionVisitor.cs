using System;
using System.Reflection.Emit;
using ILLightenComparer.Emitters.Comparisons;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using ILLightenComparer.Shared;

namespace ILLightenComparer.Emitters.Visitors.Collection
{
    internal abstract class CollectionVisitor
    {
        private readonly VariableLoader _loader;

        protected CollectionVisitor(VariableLoader loader)
        {
            _loader = loader;
        }

        protected (LocalBuilder collectionX, LocalBuilder collectionY) EmitLoad(IComparison comparison, ILEmitter il, Label gotoNext)
        {
            var variable = comparison.Variable;
            variable.Load(_loader, il, Arg.X).Store(variable.VariableType, out var collectionX);
            variable.Load(_loader, il, Arg.Y).Store(variable.VariableType, out var collectionY);

            il.EmitReferenceComparison(collectionX, collectionY, gotoNext);

            return (collectionX, collectionY);
        }

        protected static void EmitArraySorting(ILEmitter il, Type elementType, LocalBuilder xArray, LocalBuilder yArray)
        {
            // todo: compare default sorting and sorting with generated comparer - TrySZSort can work faster
            // todo: custom comparer could be defined, use it
            if (elementType.GetUnderlyingType().ImplementsGeneric(typeof(IComparable<>)))
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
