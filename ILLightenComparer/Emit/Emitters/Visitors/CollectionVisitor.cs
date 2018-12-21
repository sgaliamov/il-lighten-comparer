using System;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Comparisons;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Emitters.Visitors
{
    internal abstract class CollectionVisitor
    {
        private readonly CompareVisitor _compareVisitor;
        private readonly VariableLoader _loader;
        private readonly StackVisitor _stackVisitor;

        protected CollectionVisitor(StackVisitor stackVisitor, CompareVisitor compareVisitor, VariableLoader loader)
        {
            _compareVisitor = compareVisitor;
            _loader = loader;
            _stackVisitor = stackVisitor;
        }

        protected (LocalBuilder x, LocalBuilder y, Label gotoNext) EmitLoad(ILEmitter il, IComparison comparison)
        {
            var variable = comparison.Variable;
            variable.Load(_loader, il, Arg.X).Store(variable.VariableType, Arg.X, out var x);
            variable.Load(_loader, il, Arg.Y).Store(variable.VariableType, Arg.Y, out var y);

            il.DefineLabel(out var gotoNext)
              .EmitCheckReferenceComparison(x, y, gotoNext);

            return (x, y, gotoNext);
        }

        protected static void EmitArraySorting(
            ILEmitter il,
            Type arrayType,
            LocalBuilder xArray,
            LocalBuilder yArray)
        {
            var elementType = arrayType.GetElementType();
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

        protected void Visit(ILEmitter il, IComparisonAcceptor itemComparison, Label continueLoop)
        {
            itemComparison.LoadVariables(_stackVisitor, il, continueLoop);
            itemComparison.Accept(_compareVisitor, il)
                          .EmitReturnNotZero(continueLoop);
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
