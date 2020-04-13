using System;
using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Config;
using ILLightenComparer.Shared.Comparisons;
using ILLightenComparer.Variables;
using Illuminator;

namespace ILLightenComparer.Comparer.Comparisons
{
    internal sealed class ArraysComparison : IComparisonEmitter
    {
        private readonly IVariable _variable;
        private readonly ArrayComparer _arrayComparer;
        private readonly CollectionComparer _collectionComparer;
        private readonly IConfigurationProvider _configuration;

        private ArraysComparison(
            ComparisonResolver resolver,
            IConfigurationProvider configuration,
            IVariable variable)
        {
            _configuration = configuration;
            _variable = variable ?? throw new ArgumentNullException(nameof(variable));
            _arrayComparer = new ArrayComparer(resolver, CustomEmitters.EmitCheckIfLoopsAreDone);
            _collectionComparer = new CollectionComparer(configuration, CustomEmitters.EmitReferenceComparison);
        }

        public static ArraysComparison Create(
           ComparisonResolver comparisons,
           IConfigurationProvider configuration,
           IVariable variable)
        {
            var variableType = variable.VariableType;
            if (variableType.IsArray && variableType.GetArrayRank() == 1) {
                return new ArraysComparison(comparisons, configuration, variable);
            }

            return null;
        }

        public ILEmitter Emit(ILEmitter il, Label gotoNext)
        {
            var variableType = _variable.VariableType;

            var (x, y) = _collectionComparer.EmitLoad(_variable, il, gotoNext);
            var (countX, countY) = _arrayComparer.EmitLoadCounts(variableType, x, y, il);

            if (_configuration.Get(_variable.OwnerType).IgnoreCollectionOrder) {
                _collectionComparer.EmitArraySorting(il, variableType.GetElementType(), x, y);
            }

            return _arrayComparer.Compare(variableType, _variable.OwnerType, x, y, countX, countY, il, gotoNext);
        }

        public ILEmitter Emit(ILEmitter il)
        {
            il.DefineLabel(out var exit);

            return Emit(il, exit)
                .MarkLabel(exit)
                .Return(0);
        }

        public ILEmitter EmitCheckForIntermediateResult(ILEmitter il, Label _) => il;
    }
}
