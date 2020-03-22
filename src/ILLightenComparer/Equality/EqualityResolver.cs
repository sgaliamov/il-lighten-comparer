using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ILLightenComparer.Config;
using ILLightenComparer.Equality.Comparisons;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using ILLightenComparer.Shared;
using ILLightenComparer.Shared.Comparisons;
using ILLightenComparer.Variables;
using Illuminator.Extensions;

namespace ILLightenComparer.Equality
{
    internal sealed class EqualityResolver
    {
        private static readonly MethodInfo DelayedEquals = typeof(IEqualityComparerContext).GetMethod(nameof(IEqualityComparerContext.DelayedEquals));

        private readonly IReadOnlyCollection<Func<IVariable, IComparisonEmitter>> _comparisonFactories;
        private readonly IConfigurationProvider _configurations;

        public EqualityResolver(
            EqualityContext context,
            MembersProvider membersProvider,
            IConfigurationProvider configurations)
        {
            _configurations = configurations;

            _comparisonFactories = new Func<IVariable, IComparisonEmitter>[] {
                //(IVariable variable) => NullableComparison.Create(this, variable),
                CeqComparison.Create,
                OperatorComparison.Create,
                //ComparablesComparison.Create,
                (IVariable variable) => CreateIndirectComparison(context, variable),
                (IVariable variable) => MembersEqualityComparison.Create(this, membersProvider, variable)
                //(IVariable variable) => ArraysComparison.Create(this, _configurations, variable),
                //(IVariable variable) => EnumerablesComparison.Create(this, _configurations, variable)
            };
        }

        public IComparisonEmitter GetEqualityComparison(IVariable variable)
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

        internal static IndirectComparison CreateIndirectComparison(EqualityContext context, IVariable variable)
        {
            var variableType = variable.VariableType;
            if (variableType.IsHierarchical() && !(variable is ArgumentVariable)) {
                var delayedCompare = DelayedEquals.MakeGenericMethod(variableType);
                var staticCompareMethod = context.GetStaticEqualsMethodInfo(variableType);

                return new IndirectComparison(staticCompareMethod, delayedCompare, variable);
            }

            return null;
        }
    }
}
