using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Comparer.Comparisons;
using ILLightenComparer.Config;
using ILLightenComparer.Shared;
using ILLightenComparer.Shared.Comparisons;
using ILLightenComparer.Variables;
using Illuminator.Extensions;

namespace ILLightenComparer.Comparer
{
    internal sealed class ComparisonResolver : IResolver
    {
        private static readonly MethodInfo StringCompareMethod = typeof(string).GetMethod(
            nameof(string.Compare),
            new[] { typeof(string), typeof(string), typeof(StringComparison) });

        private static readonly MethodInfo DelayedCompare = typeof(IComparerContext).GetMethod(nameof(IComparerContext.DelayedCompare));

        private readonly IReadOnlyCollection<Func<IVariable, IComparisonEmitter>> _comparisonFactories;
        private readonly IConfigurationProvider _configuration;

        public ComparisonResolver(ComparerContext context, MembersProvider membersProvider, IConfigurationProvider configuration)
        {
            _configuration = configuration;

            var collectionComparer = new CollectionComparer(this, _configuration, CustomEmitters.EmitCheckIfLoopsAreDone, CustomEmitters.EmitReferenceComparison);

            _comparisonFactories = new Func<IVariable, IComparisonEmitter>[] {
                (IVariable variable) => NullableComparison.Create(this, CustomEmitters.EmitReturnIfTruthy, CustomEmitters.EmitCheckNullablesForValue, variable),
                IntegralsComparison.Create,
                (IVariable variable) => StringsComparison.Create(StringCompareMethod, CustomEmitters.EmitReturnIfTruthy, _configuration, variable),
                ComparablesComparison.Create,
                (IVariable variable) => MembersComparison.Create(this, 0, membersProvider, variable),
                (IVariable variable) => IndirectComparison.Create(
                    CustomEmitters.EmitReturnIfTruthy,
                    variableType => context.GetStaticCompareMethodInfo(variableType),
                    DelayedCompare,
                    variable),
                (IVariable variable) => ArraysComparison.Create(0, collectionComparer, _configuration, variable),
                (IVariable variable) => EnumerablesComparison.Create(this, 0, collectionComparer, CustomEmitters.EmitCheckIfLoopsAreDone, _configuration, variable)
            };
        }

        public IComparisonEmitter GetComparisonEmitter(IVariable variable)
        {
            var hasCustomComparer = _configuration.HasCustomComparer(variable.VariableType);
            if (hasCustomComparer) {
                return IndirectComparison.Create(CustomEmitters.EmitReturnIfTruthy, DelayedCompare, variable);
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
