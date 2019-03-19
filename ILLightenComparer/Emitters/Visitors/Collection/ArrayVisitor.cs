using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Config;
using ILLightenComparer.Emitters.Comparisons;
using ILLightenComparer.Extensions;
using ILLightenComparer.Shared;

namespace ILLightenComparer.Emitters.Visitors.Collection
{
    internal sealed class ArrayVisitor
    {
        private readonly ArrayComparer _arrayComparer;
        private readonly CollectionComparer _collectionComparer;
        private readonly IConfigurationProvider _configuration;

        public ArrayVisitor(
            Context context,
            IConfigurationProvider configuration,
            CompareVisitor compareVisitor,
            VariableLoader loader,
            Converter converter)
        {
            _configuration = configuration;
            _collectionComparer = new CollectionComparer(context, loader);
            _arrayComparer = new ArrayComparer(compareVisitor, converter);
        }

        public ILEmitter Visit(ArraysComparison comparison, ILEmitter il, Label afterLoop)
        {
            var variable = comparison.Variable;
            var variableType = variable.VariableType;

            var (x, y) = _collectionComparer.EmitLoad(comparison, il, afterLoop);
            var (countX, countY) = _arrayComparer.EmitLoadCounts(variableType, x, y, il);

            EmitCheckForNegativeCount(countX, countY, comparison.Variable.VariableType, il);

            if (_configuration.Get(variable.OwnerType).IgnoreCollectionOrder)
            {
                _collectionComparer.EmitArraySorting(il, variableType.GetElementType(), x, y);
            }

            return _arrayComparer.Compare(variableType, variable.OwnerType, x, y, countX, countY, il, afterLoop);
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
