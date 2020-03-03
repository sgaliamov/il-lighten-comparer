using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Config;
using ILLightenComparer.Emitters.Comparisons;
using Illuminator;

namespace ILLightenComparer.Emitters.Visitors.Collection
{
    internal sealed class ArrayVisitor
    {
        private readonly ArrayComparer _arrayComparer;
        private readonly CollectionComparer _collectionComparer;
        private readonly IConfigurationProvider _configurations;

        public ArrayVisitor(
            IConfigurationProvider configurations,
            CompareVisitor compareVisitor,
            VariableLoader loader,
            ComparisonsProvider comparisons)
        {
            _configurations = configurations;
            _collectionComparer = new CollectionComparer(configurations, loader);
            _arrayComparer = new ArrayComparer(compareVisitor, comparisons);
        }

        public ILEmitter Visit(ArraysComparison comparison, ILEmitter il, Label afterLoop)
        {
            var variable = comparison.Variable;
            var variableType = variable.VariableType;

            var (x, y) = _collectionComparer.EmitLoad(comparison, il, afterLoop);
            var (countX, countY) = _arrayComparer.EmitLoadCounts(variableType, x, y, il);

            if (_configurations.Get(variable.OwnerType).IgnoreCollectionOrder) {
                _collectionComparer.EmitArraySorting(il, variableType.GetElementType(), x, y);
            }

            return _arrayComparer.Compare(variableType, variable.OwnerType, x, y, countX, countY, il, afterLoop);
        }
    }
}
