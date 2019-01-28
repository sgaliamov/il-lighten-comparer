using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Comparisons;

namespace ILLightenComparer.Emit.v2.Visitors.Collection
{
    internal sealed class ArrayVisitor : CollectionVisitor
    {
        private readonly ArrayComparer _arrayComparer;
        private readonly ComparerContext _context;

        public ArrayVisitor(
            ComparerContext context,
            CompareVisitor compareVisitor,
            VariableLoader loader,
            Converter converter)
            : base(loader)
        {
            _context = context;
            _arrayComparer = new ArrayComparer(compareVisitor, converter);
        }

        public ILEmitter Visit(ArraysComparison comparison, ILEmitter il, Label afterLoop)
        {
            var variable = comparison.Variable;
            var (x, y) = EmitLoad(il, comparison);
            var (countX, countY) = _arrayComparer.EmitLoadCounts(x, y, il);

            EmitCheckForNegativeCount(countX, countY, comparison.Variable.VariableType, il);

            if (_context.GetConfiguration(variable.OwnerType).IgnoreCollectionOrder)
            {
                EmitArraySorting(il, variable.VariableType.GetElementType(), x, y);
            }

            return _arrayComparer.Compare(variable, x, y, countX, countY, il, afterLoop);
        }

        private static void EmitCheckForNegativeCount(
            LocalBuilder countX,
            LocalBuilder countY,
            MemberInfo memberType,
            ILEmitter il)
        {
            il.LoadConstant(0)
              .LoadLocal(countX)
              .Branch(OpCodes.Bgt_S, out var negativeException)
              .LoadConstant(0)
              .LoadLocal(countY)
              .Branch(OpCodes.Ble_S, out var loopInit)
              .MarkLabel(negativeException)
              .LoadString($"Collection {memberType.DisplayName()} has negative count of elements.")
              .Emit(OpCodes.Newobj, typeof(IndexOutOfRangeException).GetConstructor(new[] { typeof(string) }))
              .Emit(OpCodes.Throw)
              .MarkLabel(loopInit);
        }
    }
}
