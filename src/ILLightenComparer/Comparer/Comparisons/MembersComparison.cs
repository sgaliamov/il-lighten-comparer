using System;
using System.Linq;
using System.Reflection.Emit;
using ILLightenComparer.Config;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Comparer.Comparisons
{
    internal sealed class MembersComparison : IComparison
    {
        private readonly MembersProvider _membersProvider;
        private readonly ComparisonResolver _comparisons;
        private readonly IVariable _variable;

        private MembersComparison(
            ComparisonResolver comparisons,
            IConfigurationProvider configurations,
            IVariable variable)
        {
            _variable = variable;
            _comparisons = comparisons;
            _membersProvider = new MembersProvider(configurations);
        }

        public static MembersComparison Create(
            ComparisonResolver comparisons,
            IConfigurationProvider configurations,
            IVariable variable)
        {
            if (variable.VariableType.IsHierarchical() && variable is ArgumentVariable) {
                return new MembersComparison(comparisons, configurations, variable);
            }

            return null;
        }

        public bool PutsResultInStack => false;

        public ILEmitter Emit(ILEmitter il, Label _)
        {
            var variableType = _variable.VariableType;
            if (variableType.IsPrimitive()) {
                throw new InvalidOperationException($"{variableType.DisplayName()} is not expected.");
            }

            var comparisons = _membersProvider
                              .GetMembers(variableType)
                              .Select(_comparisons.GetComparison);

            foreach (var item in comparisons) {
                using (il.LocalsScope()) {
                    il.DefineLabel(out var gotoNext);

                    item.Emit(il, gotoNext);

                    if (item.PutsResultInStack) {
                        il.EmitReturnNotZero(gotoNext);
                    }

                    il.MarkLabel(gotoNext);
                }
            }

            return il.LoadInteger(0);
        }

        public ILEmitter Emit(ILEmitter il) => Emit(il, default).Return();
    }
}
