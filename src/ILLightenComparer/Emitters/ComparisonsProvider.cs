using System;
using System.Linq;
using ILLightenComparer.Config;
using ILLightenComparer.Emitters.Comparisons;
using ILLightenComparer.Emitters.Variables;
using ILLightenComparer.Extensions;

namespace ILLightenComparer.Emitters
{
    internal sealed class ComparisonsProvider
    {
        private static readonly Func<IVariable, IComparison>[] ComparisonConverters = {
            NullableComparison.Create,
            IntegralsComparison.Create,
            StringsComparison.Create,
            ComparablesComparison.Create,
            MembersComparison.Create,
            HierarchicalsComparison.Create,
            ArraysComparison.Create,
            EnumerablesComparison.Create
        };

        private readonly IConfigurationProvider _configurations;

        public ComparisonsProvider(IConfigurationProvider configurations) => _configurations = configurations;

        public IComparison GetComparison(IVariable variable) {
            var hasCustomComparer = _configurations.HasCustomComparer(variable.VariableType);
            if (hasCustomComparer) {
                return new CustomComparison(variable);
            }

            var comparison = ComparisonConverters
                             .Select(factory => factory(variable))
                             .FirstOrDefault(x => x != null);

            if (comparison == null) {
                throw new NotSupportedException($"{variable.VariableType.DisplayName()} is not supported.");
            }

            return comparison;
        }
    }
}
