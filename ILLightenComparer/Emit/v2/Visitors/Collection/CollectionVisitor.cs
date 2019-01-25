using System;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Comparisons;

namespace ILLightenComparer.Emit.v2.Visitors.Collection
{
    internal abstract class CollectionVisitor
    {
        protected const int LocalX = Arg.X; // 1
        protected const int LocalY = Arg.Y; // 2

        private readonly CompareVisitor _compareVisitor;
        private readonly Converter _converter;
        private readonly VariableLoader _loader;
        private readonly VariablesVisitor _variablesVisitor;

        protected CollectionVisitor(
            VariablesVisitor variablesVisitor,
            CompareVisitor compareVisitor,
            VariableLoader loader,
            Converter converter)
        {
            _compareVisitor = compareVisitor;
            _converter = converter;
            _loader = loader;
            _variablesVisitor = variablesVisitor;
        }

        protected (LocalBuilder collectionX, LocalBuilder collectionY, Label gotoNext) EmitLoad(ILEmitter il, IVariableComparison comparison)
        {
            var variable = comparison.Variable;
            variable.Load(_loader, il, Arg.X).Store(variable.VariableType, LocalX, out var collectionX);
            variable.Load(_loader, il, Arg.Y).Store(variable.VariableType, LocalY, out var collectionY);

            il.DefineLabel(out var gotoNext)
              .EmitReferenceComparison(collectionX, collectionY, gotoNext);

            return (collectionX, collectionY, gotoNext);
        }

        protected void Visit(ILEmitter il, IComparison itemVisitors, Type elementType, Label continueLoop)
        {
            if (elementType.IsNullable())
            {
                var variable = itemVisitors.Variable;
                var variableType = variable.VariableType;

                variable.Load(_loader, il, Arg.X).Store(variableType, LocalX, out var nullableX);
                variable.Load(_loader, il, Arg.Y).Store(variableType, LocalY, out var nullableY);
                il.EmitCheckNullablesForValue(nullableX, nullableY, variableType, continueLoop);

                itemVisitors = _converter.CreateNullableVariableComparison(variable, nullableX, nullableY);
            }

            itemVisitors.Accept(_compareVisitor, il)
                          .EmitReturnNotZero(continueLoop);
        }

        protected static void EmitArraySorting(ILEmitter il, Type elementType, LocalBuilder xArray, LocalBuilder yArray)
        {
            // todo: compare default sorting and sorting with generated comparer - TrySZSort can work faster
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
