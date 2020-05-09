using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Config;
using ILLightenComparer.Equality.Comparisons;
using ILLightenComparer.Shared;
using ILLightenComparer.Shared.Comparisons;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Equality
{
    internal sealed class EqualityResolver : IResolver
    {
        private static readonly MethodInfo StringEqualsMethod = typeof(string).GetMethod(
            nameof(string.Equals),
            new[] { typeof(string), typeof(string), typeof(StringComparison) });

        private static readonly MethodInfo DelayedEquals = typeof(IEqualityComparerContext)
            .GetMethod(nameof(IEqualityComparerContext.DelayedEquals));

        private readonly IReadOnlyCollection<Func<IVariable, IComparisonEmitter>> _comparisonFactories;
        private readonly IConfigurationProvider _configuration;

        public EqualityResolver(
            EqualityContext context,
            MembersProvider membersProvider,
            IConfigurationProvider configuration)
        {
            _configuration = configuration;

            var collectionComparer = new ArrayComparisonEmitter(this, CustomEmitters.EmitCheckIfLoopsAreDone, CustomEmitters.EmitReferenceComparison);

            _comparisonFactories = new Func<IVariable, IComparisonEmitter>[] {
                (IVariable variable) => NullableComparison.Create(this, CustomEmitters.EmitReturnIfFalsy, CustomEmitters.EmitCheckNullablesForValue, variable),
                CeqEqualityComparison.Create,
                (IVariable variable) => StringsComparison.Create(StringEqualsMethod, CustomEmitters.EmitReturnIfFalsy, _configuration, variable),
                OperatorEqualityComparison.Create,
                (IVariable variable) => MembersComparison.Create(this, membersProvider, variable),
                (IVariable variable) => IndirectComparison.Create(
                    CustomEmitters.EmitReturnIfFalsy,
                    variableType => context.GetStaticEqualsMethodInfo(variableType),
                    DelayedEquals,
                    variable),
                (IVariable variable) => ArraysComparison.Create(collectionComparer, _configuration, variable),
                (IVariable variable) => EnumerablesComparison.Create(this, collectionComparer, CustomEmitters.EmitCheckIfLoopsAreDone, _configuration, variable)
            };
        }

        public void EmitCheckForIntermediateResult(ILEmitter il, Label next) => il.IfTrue(next).Return(0);

        public IComparisonEmitter GetComparisonEmitter(IVariable variable)
        {
            var hasCustomComparer = _configuration.HasCustomEqualityComparer(variable.VariableType);
            if (hasCustomComparer) {
                return IndirectComparison.Create(CustomEmitters.EmitReturnIfFalsy, DelayedEquals, variable);
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
