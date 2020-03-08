using System;
using System.Collections.Generic;
using System.Linq;
using ILLightenComparer.Comparer.Builders;
using ILLightenComparer.Comparer.Comparisons;
using ILLightenComparer.Config;
using ILLightenComparer.Variables;
using Illuminator.Extensions;

namespace ILLightenComparer.Comparer
{
    internal sealed class ComparisonResolver
    {
        private readonly IReadOnlyCollection<Func<IVariable, IComparison>> _converters;
        private readonly IConfigurationProvider _configurations;

        public ComparisonResolver(
            ComparerProvider provider,
            IConfigurationProvider configurations)
        {
            _configurations = configurations;
            _converters = new Func<IVariable, IComparison>[] {
                (IVariable variable) => NullableComparison.Create(this, variable),
                IntegralsComparison.Create,
                (IVariable variable) => StringsComparison.Create(_configurations, variable),
                ComparablesComparison.Create,
                (IVariable variable) => MembersComparison.Create(this, _configurations, variable),
                (IVariable variable) => HierarchicalsComparison.Create(provider, variable),
                (IVariable variable) => ArraysComparison.Create(this, _configurations, variable),
                (IVariable variable) => EnumerablesComparison.Create(this, _configurations, variable)
            };
        }

        public IComparison GetComparison(IVariable variable)
        {
            var hasCustomComparer = _configurations.HasCustomComparer(variable.VariableType);
            if (hasCustomComparer) {
                return new CustomComparison(variable);
            }

            var comparison = _converters
                .Select(factory => factory(variable))
                .FirstOrDefault(x => x != null);

            if (comparison == null) {
                throw new NotSupportedException($"{variable.VariableType.DisplayName()} is not supported.");
            }

            return comparison;
        }
    }
}
