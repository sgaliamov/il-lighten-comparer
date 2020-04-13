using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Extensions;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Shared.Comparisons
{
    internal sealed class MembersComparison : IComparisonEmitter
    {
        private readonly MembersProvider _membersProvider;
        private readonly IResolver _resolver;
        private readonly int _defaultResult;
        private readonly IVariable _variable;

        private MembersComparison(
            IResolver resolver,
            int defaultResult,
            MembersProvider membersProvider,
            IVariable variable)
        {
            _variable = variable;
            _resolver = resolver;
            _defaultResult = defaultResult;
            _membersProvider = membersProvider;
        }

        public static MembersComparison Create(
            IResolver resolver,
            int defaultResult,
            MembersProvider membersProvider,
            IVariable variable)
        {
            if (variable.VariableType.IsHierarchical() && variable is ArgumentVariable) {
                return new MembersComparison(resolver, defaultResult, membersProvider, variable);
            }

            return null;
        }

        public ILEmitter Emit(ILEmitter il, Label _)
        {
            var variableType = _variable.VariableType;
            Debug.Assert(!variableType.IsPrimitive(), $"{variableType.DisplayName()} is not expected.");

            var comparisons = _membersProvider
                .GetMembers(variableType)
                .Select(_resolver.GetComparisonEmitter)
                .ToArray();

            for (var i = 0; i < comparisons.Length - 1; i++) {
                using (il.LocalsScope()) {
                    il.DefineLabel(out var gotoNext);
                    comparisons[i].Emit(il, gotoNext);
                    comparisons[i].EmitCheckForIntermediateResult(il, gotoNext);
                    il.MarkLabel(gotoNext);
                }
            }

            var last = comparisons.LastOrDefault();
            if (last is null) {
                il.Return(_defaultResult);
            } else {
                last.Emit(il);
            }

            return il;
        }

        public ILEmitter Emit(ILEmitter il) => Emit(il, default);

        public ILEmitter EmitCheckForIntermediateResult(ILEmitter _, Label __) => throw new NotSupportedException($"{nameof(IndirectComparison)} should be used.");
    }
}
