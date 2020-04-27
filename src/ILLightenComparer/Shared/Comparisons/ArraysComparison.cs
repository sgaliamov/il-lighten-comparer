using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Config;
using ILLightenComparer.Extensions;
using ILLightenComparer.Variables;
using Illuminator;

namespace ILLightenComparer.Shared.Comparisons
{
    internal sealed class ArraysComparison : IComparisonEmitter
    {
        private readonly int _defaultResult;
        private readonly ArrayComparisonEmitter _collectionComparer;
        private readonly IConfigurationProvider _configuration;
        private readonly IVariable _variable;

        private ArraysComparison(int defaultResult, ArrayComparisonEmitter collectionComparer, IConfigurationProvider configuration, IVariable variable)
        {
            _configuration = configuration;
            _variable = variable;
            _defaultResult = defaultResult;
            _collectionComparer = collectionComparer;
        }

        public static ArraysComparison Create(int defaultResult, ArrayComparisonEmitter collectionComparer, IConfigurationProvider configuration, IVariable variable)
        {
            var variableType = variable.VariableType;
            if (variableType.IsArray && variableType.GetArrayRank() == 1) {
                return new ArraysComparison(defaultResult, collectionComparer, configuration, variable);
            }

            return null;
        }

        public ILEmitter Emit(ILEmitter il, Label gotoNext)
        {
            var variableType = _variable.VariableType;
            var (x, y) = _collectionComparer.EmitLoad(_variable, il, gotoNext);
            var (countX, countY) = il.EmitLoadCounts(variableType, x, y);

            if (_configuration.Get(_variable.OwnerType).IgnoreCollectionOrder) {
                il.EmitArraySorting(_configuration, variableType.GetElementType(), x, y);
            }

            return _collectionComparer.EmitCompareArrays(variableType, _variable.OwnerType, x, y, countX, countY, il, gotoNext);
        }

        public ILEmitter Emit(ILEmitter il) => il
            .DefineLabel(out var exit)
            .Execute(this.Emit(exit))
            .MarkLabel(exit)
            .Return(_defaultResult);

        public ILEmitter EmitCheckForIntermediateResult(ILEmitter il, Label _) => il;
    }
}
