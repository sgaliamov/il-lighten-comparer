using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using ILLightenComparer.Shared;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Comparer.Comparisons
{
    internal sealed class MembersComparison : IStepEmitter
    {
        private readonly MembersProvider _membersProvider;
        private readonly ComparisonResolver _resolver;
        private readonly IVariable _variable;

        private MembersComparison(
            ComparisonResolver resolver,
            MembersProvider membersProvider,
            IVariable variable)
        {
            _variable = variable;
            _resolver = resolver;
            _membersProvider = membersProvider;
        }

        public static MembersComparison Create(
            ComparisonResolver resolver,
            MembersProvider membersProvider,
            IVariable variable)
        {
            if (variable.VariableType.IsHierarchical() && variable is ArgumentVariable) {
                return new MembersComparison(resolver, membersProvider, variable);
            }

            return null;
        }

        public bool PutsResultInStack { get; }

        public ILEmitter Emit(ILEmitter il, Label _)
        {
            var variableType = _variable.VariableType;
            Debug.Assert(!variableType.IsPrimitive(), $"{variableType.DisplayName()} is not expected.");

            var comparisons = _membersProvider
                .GetMembers(variableType)
                .Select(_resolver.GetComparison);

            foreach (var item in comparisons) {
                using (il.LocalsScope()) {
                    il.DefineLabel(out var gotoNext);

                    item.Emit(il, gotoNext);

                    if (item.PutsResultInStack) {
                        il.EmitReturnIfNonZero(gotoNext);
                    }

                    il.MarkLabel(gotoNext);
                }
            }

            return il.LoadInteger(0);
        }

        public ILEmitter Emit(ILEmitter il) => Emit(il, default).Return();
    }
}
