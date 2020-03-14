using System;
using System.Collections.Generic;
using System.Linq;
using ILLightenComparer.Config;
using ILLightenComparer.Equality.Comparisons;
using ILLightenComparer.Shared;
using ILLightenComparer.Shared.Comparisons;
using ILLightenComparer.Variables;
using Illuminator.Extensions;

namespace ILLightenComparer.Equality
{
    internal sealed class EqualityResolver
    {
        private readonly IReadOnlyCollection<Func<IVariable, IStepEmitter>> _converters;
        private readonly IConfigurationProvider _configurations;

        public EqualityResolver(EqualityContext context, IConfigurationProvider configurations)
        {
            _configurations = configurations;
            _converters = new Func<IVariable, IStepEmitter>[] {
                CeqComparison.Create,
                OperatorComparison.Create
            };
        }

        public IStepEmitter GetComparison(IVariable variable)
        {
            var hasCustomComparer = _configurations.HasCustomEqualityComparer(variable.VariableType);
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
