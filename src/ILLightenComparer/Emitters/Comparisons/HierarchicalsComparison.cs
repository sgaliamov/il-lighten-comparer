using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emitters.Builders;
using ILLightenComparer.Emitters.Variables;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using Illuminator;

namespace ILLightenComparer.Emitters.Comparisons
{
    internal sealed class HierarchicalsComparison : IComparison
    {
        private readonly ComparerProvider _context;
        private readonly IVariable _variable;
        private readonly MethodInfo _delayedCompare;

        private HierarchicalsComparison(ComparerProvider context, IVariable variable)
        {
            _context = context;
            _variable = variable;
            _delayedCompare = Method.DelayedCompare.MakeGenericMethod(_variable.VariableType);
        }

        public static HierarchicalsComparison Create(ComparerProvider provider, IVariable variable)
        {
            if (variable.VariableType.IsHierarchical() && !(variable is ArgumentVariable)) {
                return new HierarchicalsComparison(provider, variable);
            }

            return null;
        }

        public bool PutsResultInStack => true;

        public ILEmitter Compare(ILEmitter il, Label _)
        {
            var variable = _variable;
            var variableType = variable.VariableType;

            il.LoadArgument(Arg.Context);
            variable.Load(il, Arg.X);
            variable.Load(il, Arg.Y);
            il.LoadArgument(Arg.SetX)
              .LoadArgument(Arg.SetY);

            var typeOfVariableCanBeChangedOnRuntime =
                !variableType.IsValueType && !variableType.IsSealed;

            if (typeOfVariableCanBeChangedOnRuntime) {
                return il.Call(_delayedCompare);
            }

            var compareMethod = _context.GetStaticCompareMethodInfo(variableType);

            return il.Call(compareMethod);
        }

        public ILEmitter Compare(ILEmitter il) => Compare(il, default).Return();
    }
}
