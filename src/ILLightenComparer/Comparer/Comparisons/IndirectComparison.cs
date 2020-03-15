using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using ILLightenComparer.Shared;
using ILLightenComparer.Variables;
using Illuminator;

namespace ILLightenComparer.Comparer.Comparisons
{
    /// <summary>
    /// Delegates comparison to static method or delayed compare in context.
    /// </summary>
    internal sealed class IndirectComparison : IStepEmitter
    {
        private readonly IVariable _variable;
        private readonly MethodInfo _delayedCompare;
        private readonly ComparerContext _context;

        private IndirectComparison(ComparerContext context, IVariable variable)
        {
            _context = context;
            _variable = variable;
            _delayedCompare = Method.DelayedCompare.MakeGenericMethod(_variable.VariableType);
        }

        public static IndirectComparison Create(ComparerContext context, IVariable variable)
        {
            if (variable.VariableType.IsHierarchical() && !(variable is ArgumentVariable)) {
                return new IndirectComparison(context, variable);
            }

            return null;
        }

        public bool PutsResultInStack { get; } = true;

        public ILEmitter Emit(ILEmitter il, Label _)
        {
            var variableType = _variable.VariableType;

            il.LoadArgument(Arg.Context);
            _variable.Load(il, Arg.X);
            _variable.Load(il, Arg.Y);
            il.LoadArgument(Arg.SetX)
              .LoadArgument(Arg.SetY);

            var typeOfVariableCanBeChangedOnRuntime = !variableType.IsSealedType();
            if (typeOfVariableCanBeChangedOnRuntime) {
                return il.Call(_delayedCompare);
            }

            var compareMethod = _context.GetStaticCompareMethodInfo(variableType);

            return il.Call(compareMethod);
        }

        public ILEmitter Emit(ILEmitter il) => Emit(il, default).Return();
    }
}
