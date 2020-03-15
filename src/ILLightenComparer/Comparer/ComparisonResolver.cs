using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ILLightenComparer.Comparer.Comparisons;
using ILLightenComparer.Config;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using ILLightenComparer.Shared;
using ILLightenComparer.Shared.Comparisons;
using ILLightenComparer.Variables;
using Illuminator.Extensions;

namespace ILLightenComparer.Comparer
{
    internal sealed class ComparisonResolver
    {
        private static readonly MethodInfo DelayedCompare = typeof(IComparerContext).GetMethod(nameof(IComparerContext.DelayedCompare));
        private readonly IReadOnlyCollection<Func<IVariable, IStepEmitter>> _comparisonFactories;
        private readonly IConfigurationProvider _configurations;

        public ComparisonResolver(
            ComparerContext context,
            MembersProvider membersProvider,
            IConfigurationProvider configurations)
        {
            _configurations = configurations;

            _comparisonFactories = new Func<IVariable, IStepEmitter>[] {
                (IVariable variable) => NullableComparison.Create(this, variable),
                IntegralsComparison.Create,
                (IVariable variable) => StringsComparison.Create(_configurations, variable),
                ComparablesComparison.Create,
                (IVariable variable) => MembersComparison.Create(this, membersProvider, variable),
                (IVariable variable) => CreateIndirectComparison(context, variable),
                (IVariable variable) => ArraysComparison.Create(this, _configurations, variable),
                (IVariable variable) => EnumerablesComparison.Create(this, _configurations, variable)
            };
        }

        public IStepEmitter GetComparison(IVariable variable)
        {
            var hasCustomComparer = _configurations.HasCustomComparer(variable.VariableType);
            if (hasCustomComparer) {
                return new CustomComparison(variable, DelayedCompare);
            }

            var comparison = _comparisonFactories
                .Select(factory => factory(variable))
                .FirstOrDefault(x => x != null);

            if (comparison == null) {
                throw new NotSupportedException($"{variable.VariableType.DisplayName()} is not supported.");
            }

            return comparison;
        }

        internal static IndirectComparison CreateIndirectComparison(ComparerContext context, IVariable variable)
        {
            var variableType = variable.VariableType;
            if (variableType.IsHierarchical() && !(variable is ArgumentVariable)) {
                var delayedCompare = DelayedCompare.MakeGenericMethod(variableType);
                var staticCompareMethod = context.GetStaticCompareMethodInfo(variableType);

                return new IndirectComparison(staticCompareMethod, delayedCompare, variable);
            }

            return null;
        }
    }
}
