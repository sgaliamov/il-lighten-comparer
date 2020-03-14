using System;
using System.Collections.Generic;
using System.Linq;
using ILLightenComparer.Comparer.Comparisons;
using ILLightenComparer.Config;
using ILLightenComparer.Reflection;
using ILLightenComparer.Shared;
using ILLightenComparer.Shared.Comparisons;
using ILLightenComparer.Variables;
using Illuminator.Extensions;

namespace ILLightenComparer.Comparer
{
    internal sealed class ComparisonResolver
    {
        private readonly IReadOnlyCollection<Func<IVariable, IStepEmitter>> _comparisonFactories;
        private readonly IConfigurationProvider _configurations;

        public ComparisonResolver(ComparerContext context, IConfigurationProvider configurations)
        {
            _configurations = configurations;
            _comparisonFactories = new Func<IVariable, IStepEmitter>[] {
                (IVariable variable) => NullableComparison.Create(this, variable),
                IntegralsComparison.Create,
                (IVariable variable) => StringsComparison.Create(_configurations, variable),
                ComparablesComparison.Create,
                (IVariable variable) => MembersComparison.Create(this, _configurations, variable),
                (IVariable variable) => IndirectComparison.Create(context, variable),
                (IVariable variable) => ArraysComparison.Create(this, _configurations, variable),
                (IVariable variable) => EnumerablesComparison.Create(this, _configurations, variable)
            };
        }

        public IStepEmitter GetComparison(IVariable variable)
        {
            var hasCustomComparer = _configurations.HasCustomComparer(variable.VariableType);
            if (hasCustomComparer) {
                return new CustomComparison(variable, Method.DelayedCompare);
            }

            var comparison = _comparisonFactories
                .Select(factory => factory(variable))
                .FirstOrDefault(x => x != null);

            if (comparison == null) {
                throw new NotSupportedException($"{variable.VariableType.DisplayName()} is not supported.");
            }

            return comparison;
        }
    }
}
