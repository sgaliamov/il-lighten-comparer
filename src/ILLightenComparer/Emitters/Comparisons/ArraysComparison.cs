using System;
using System.Reflection.Emit;
using ILLightenComparer.Config;
using ILLightenComparer.Emitters.Variables;
using ILLightenComparer.Emitters.Visitors.Collection;
using Illuminator;

namespace ILLightenComparer.Emitters.Comparisons
{
    internal sealed class ArraysComparison : IComparison
    {
        private readonly ArrayVisitor _arrayVisitor;

        private ArraysComparison(
            ComparisonResolver comparisons,
            IConfigurationProvider configurations,
            IVariable variable)
        {
            Variable = variable ?? throw new ArgumentNullException(nameof(variable));

            _arrayVisitor = new ArrayVisitor(configurations, comparisons);
        }

        public IVariable Variable { get; }
        public bool PutsResultInStack => false;

        public ILEmitter Accept(ILEmitter il, Label gotoNext) => _arrayVisitor.Visit(this, il, gotoNext);

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

        public static ArraysComparison Create(
            ComparisonResolver comparisons,
            IConfigurationProvider configurations,
            IVariable variable)
        {
            var variableType = variable.VariableType;
            if (variableType.IsArray && variableType.GetArrayRank() == 1) {
                return new ArraysComparison(comparisons, configurations, variable);
            }

            return null;
        }
    }
}
