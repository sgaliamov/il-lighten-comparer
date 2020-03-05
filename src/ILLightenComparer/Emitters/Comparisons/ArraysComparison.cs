using System;
using System.Reflection.Emit;
using ILLightenComparer.Config;
using ILLightenComparer.Emitters.Comparisons.Collection;
using ILLightenComparer.Emitters.Variables;
using Illuminator;

namespace ILLightenComparer.Emitters.Comparisons
{
    internal sealed class ArraysComparison : IComparison
    {
        private readonly IVariable _variable;
        private readonly ArrayComparer _arrayComparer;
        private readonly CollectionComparer _collectionComparer;
        private readonly IConfigurationProvider _configurations;

        private ArraysComparison(
            ComparisonResolver comparisons,
            IConfigurationProvider configurations,
            IVariable variable)
        {
            _configurations = configurations;
            _variable = variable ?? throw new ArgumentNullException(nameof(variable));
            _arrayComparer = new ArrayComparer(comparisons);
            _collectionComparer = new CollectionComparer(configurations);
        }

        public static ArraysComparison Create(
           ComparisonResolver comparisons,
           IConfigurationProvider configurations,
           IVariable variable)
        {
            var variableType = variable.VariableType;
            if (variableType.IsArray && variableType.GetArrayRank() == 1) {
                return new ArraysComparison(comparisons, configurations, variable);
            }

            return null;
        }

        public bool PutsResultInStack => false;

        public ILEmitter Compare(ILEmitter il, Label gotoNext)
        {
            var variableType = _variable.VariableType;

            var (x, y) = _collectionComparer.EmitLoad(_variable, il, gotoNext);
            var (countX, countY) = _arrayComparer.EmitLoadCounts(variableType, x, y, il);

            if (_configurations.Get(_variable.OwnerType).IgnoreCollectionOrder) {
                _collectionComparer.EmitArraySorting(il, variableType.GetElementType(), x, y);
            }

            return _arrayComparer.Compare(variableType, _variable.OwnerType, x, y, countX, countY, il, gotoNext);
        }

        public ILEmitter Compare(ILEmitter il)
        {
            il.DefineLabel(out var exit);

            return Compare(il, exit)
                .MarkLabel(exit)
                .Return(0);
        }
    }
}
