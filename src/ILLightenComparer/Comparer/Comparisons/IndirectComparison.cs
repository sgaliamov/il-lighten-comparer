using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using ILLightenComparer.Variables;
using Illuminator;

namespace ILLightenComparer.Comparer.Comparisons
{
    /// <summary>
    /// Delegates comparison to static method or delayed compare in context.
    /// </summary>
    internal sealed class IndirectComparison : IComparison
    {
        private readonly ComparerProvider _context;
        private readonly IVariable _variable;
        private readonly MethodInfo _delayedCompare;

        private IndirectComparison(ComparerProvider context, IVariable variable)
        {
            _context = context;
            _variable = variable;
            _delayedCompare = Method.DelayedCompare.MakeGenericMethod(_variable.VariableType);
        }

        public static IndirectComparison Create(ComparerProvider provider, IVariable variable)
        {
            if (variable.VariableType.IsHierarchical() && !(variable is ArgumentVariable)) {
                return new IndirectComparison(provider, variable);
            }

            return null;
        }

        public bool PutsResultInStack => true;

        public ILEmitter Emit(ILEmitter il, Label _)
        {
            var variableType = _variable.VariableType;

            il.LoadArgument(Arg.Context);
            _variable.Load(il, Arg.X);
            _variable.Load(il, Arg.Y);
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

        public ILEmitter Emit(ILEmitter il) => Emit(il, default).Return();
    }
}
