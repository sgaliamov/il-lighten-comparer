using System;
using System.Collections.Generic;
using System.Linq;
using ILLightenComparer.Config;
using ILLightenComparer.Equality.Comparisons;
using ILLightenComparer.Equality.Hashers;
using ILLightenComparer.Reflection;
using ILLightenComparer.Shared;
using ILLightenComparer.Shared.Comparisons;
using ILLightenComparer.Variables;
using Illuminator.Extensions;

namespace ILLightenComparer.Equality
{
    internal sealed class EqualityResolver
    {
        private readonly IReadOnlyCollection<Func<IVariable, IStepEmitter>> _comparisonFactories;
        private readonly IReadOnlyCollection<Func<IVariable, IStepEmitter>> _hashersFactories;
        private readonly IConfigurationProvider _configurations;

        public EqualityResolver(
            EqualityContext context,
            MembersProvider membersProvider,
            IConfigurationProvider configurations)
        {
            _configurations = configurations;

            _comparisonFactories = new Func<IVariable, IStepEmitter>[] {
                CeqComparison.Create,
                OperatorComparison.Create
            };

            _hashersFactories = new Func<IVariable, IStepEmitter>[] {
            };
        }

        public IStepEmitter GetComparison(IVariable variable)
        {
            var hasCustomComparer = _configurations.HasCustomEqualityComparer(variable.VariableType);
            if (hasCustomComparer) {
                return new CustomComparison(variable, Method.DelayedEquals);
            }

            var comparison = _comparisonFactories
                .Select(factory => factory(variable))
                .FirstOrDefault(x => x != null);

            if (comparison == null) {
                throw new NotSupportedException($"{variable.VariableType.DisplayName()} is not supported.");
            }

            return comparison;
        }

        public IStepEmitter GetHasher(IVariable variable)
        {
            var hasCustomComparer = _configurations.HasCustomEqualityComparer(variable.VariableType);
            if (hasCustomComparer) {
                return new CustomHasher(variable, Method.DelayedHash);
            }

            var hasher = _hashersFactories
                .Select(factory => factory(variable))
                .FirstOrDefault(x => x != null);

            if (hasher == null) {
                throw new NotSupportedException($"{variable.VariableType.DisplayName()} is not supported.");
            }

            return hasher;
        }
    }
}
