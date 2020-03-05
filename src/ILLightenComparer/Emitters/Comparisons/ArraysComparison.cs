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
        private readonly ArrayComparer _arrayComparer;
        private readonly CollectionComparer _collectionComparer;
        private readonly IConfigurationProvider _configurations;

        private ArraysComparison(
            ComparisonResolver comparisons,
            IConfigurationProvider configurations,
            IVariable variable)
        {
            _configurations = configurations;
            Variable = variable ?? throw new ArgumentNullException(nameof(variable));
            _arrayComparer = new ArrayComparer(comparisons);
            _collectionComparer = new CollectionComparer(configurations);
        }

        public IVariable Variable { get; }
        public bool PutsResultInStack => false;

        public ILEmitter Accept(ILEmitter il, Label gotoNext)
        {
            var variableType = Variable.VariableType;

            var (x, y) = _collectionComparer.EmitLoad(this, il, gotoNext);
            var (countX, countY) = _arrayComparer.EmitLoadCounts(variableType, x, y, il);

            if (_configurations.Get(Variable.OwnerType).IgnoreCollectionOrder) {
                _collectionComparer.EmitArraySorting(il, variableType.GetElementType(), x, y);
            }

            return _arrayComparer.Compare(variableType, Variable.OwnerType, x, y, countX, countY, il, gotoNext);
        }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

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
    }
}
