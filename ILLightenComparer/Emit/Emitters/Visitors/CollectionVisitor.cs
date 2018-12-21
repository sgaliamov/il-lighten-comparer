using System;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Comparisons;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Emitters.Visitors
{
    internal abstract class CollectionVisitor
    {
        protected const int LocalX = Arg.X; // 1
        protected const int LocalY = Arg.Y; // 2

        private readonly CompareVisitor _compareVisitor;
        private readonly Converter _converter;
        private readonly VariableLoader _loader;
        private readonly StackVisitor _stackVisitor;

        protected CollectionVisitor(
            StackVisitor stackVisitor,
            CompareVisitor compareVisitor,
            VariableLoader loader,
            Converter converter)
        {
            _compareVisitor = compareVisitor;
            _converter = converter;
            _loader = loader;
            _stackVisitor = stackVisitor;
        }

        protected (LocalBuilder x, LocalBuilder y, Label gotoNext) EmitLoad(ILEmitter il, IComparison comparison)
        {
            var variable = comparison.Variable;
            variable.Load(_loader, il, Arg.X).Store(variable.VariableType, LocalX, out var x);
            variable.Load(_loader, il, Arg.Y).Store(variable.VariableType, LocalY, out var y);

            il.DefineLabel(out var gotoNext)
              .EmitCheckReferenceComparison(x, y, gotoNext);

            return (x, y, gotoNext);
        }

        protected static void EmitArraySorting(
            ILEmitter il,
            Type elementType,
            LocalBuilder xArray,
            LocalBuilder yArray)
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

        protected void Visit(ILEmitter il, IComparisonAcceptor itemComparison, Type elementType, Label continueLoop)
        {
            if (elementType.IsNullable())
            {
                VisitNullable(il, itemComparison.Variable, continueLoop);
            }
            else
            {
                Visit(il, itemComparison, continueLoop);
            }
        }

        private void Visit(ILEmitter il, IComparisonAcceptor itemComparison, Label continueLoop)
        {
            itemComparison.LoadVariables(_stackVisitor, il, continueLoop);
            itemComparison.Accept(_compareVisitor, il)
                          .EmitReturnNotZero(continueLoop);
        }

        private void VisitNullable(ILEmitter il, IVariable itemVariable, Label continueLoop)
        {
            itemVariable.Load(_loader, il, Arg.X);
            il.Store(itemVariable.VariableType, LocalX, out var nullableX);
            itemVariable.Load(_loader, il, Arg.Y);
            il.Store(itemVariable.VariableType, LocalY, out var nullableY);
            il.CheckNullableValuesForNull(nullableX, nullableY, itemVariable.VariableType, continueLoop);

            var itemComparison = _converter.CreateNullableVariableComparison(itemVariable, nullableX, nullableY);

            Visit(il, itemComparison, continueLoop);
        }

        private static void EmitSortArray(ILEmitter il, Type elementType, LocalBuilder array, LocalBuilder comparer)
        {
            var copyMethod = Method.ToArray.MakeGenericMethod(elementType);
            var sortMethod = Method.GetArraySortWithComparer(elementType);

            // todo: test that compared objects are not mutated
            il.LoadLocal(array)
              .Call(copyMethod)
              .Store(array)
              .LoadLocal(array)
              .LoadLocal(comparer)
              .Call(sortMethod);
        }

        private static void EmitSortArray(
            ILEmitter il,
            Type elementType,
            LocalBuilder array)
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
