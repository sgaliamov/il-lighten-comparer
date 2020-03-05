using System;
using System.Linq;
using System.Reflection.Emit;
using ILLightenComparer.Config;
using ILLightenComparer.Emitters.Variables;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Emitters.Comparisons
{
    internal sealed class MembersComparison : IComparison
    {
        private readonly MembersProvider _membersProvider;
        private readonly ComparisonResolver _comparisons;

        private MembersComparison(
            ComparisonResolver comparisons,
            IConfigurationProvider configurations,
            IVariable variable)
        {
            Variable = variable;
            _comparisons = comparisons;
            _membersProvider = new MembersProvider(configurations);
        }

        public IVariable Variable { get; }
        public bool PutsResultInStack => throw new NotSupportedException();

        public ILEmitter Accept(ILEmitter il, Label _)
        {
            var variableType = Variable.VariableType;
            if (variableType.IsPrimitive()) {
                throw new InvalidOperationException($"{variableType.DisplayName()} is not expected.");
            }

            var comparisons = _membersProvider
                              .GetMembers(variableType)
                              .Select(_comparisons.GetComparison);

            foreach (var item in comparisons) {
                using (il.LocalsScope()) {
                    il.DefineLabel(out var gotoNext);

                    item.Accept(il, gotoNext);

                    if (item.PutsResultInStack) {
                        il.EmitReturnNotZero(gotoNext);
                    }

                    il.MarkLabel(gotoNext);
                }
            }

            return il.LoadInteger(0);
        }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

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
    }
}
