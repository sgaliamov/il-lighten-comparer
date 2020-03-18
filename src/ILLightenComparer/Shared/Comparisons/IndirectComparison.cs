using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Extensions;
using ILLightenComparer.Variables;
using Illuminator;

namespace ILLightenComparer.Shared.Comparisons
{
    /// <summary>
    ///     Delegates comparison to static method or delayed compare in context.
    /// </summary>
    internal sealed class IndirectComparison : IComparisonEmitter
    {
        private readonly MethodInfo _staticCompareMethod;
        private readonly IVariable _variable;
        private readonly MethodInfo _delayedCompare;

        public IndirectComparison(MethodInfo staticCompareMethod, MethodInfo delayedCompare, IVariable variable)
        {
            _staticCompareMethod = staticCompareMethod;
            _delayedCompare = delayedCompare;
            _variable = variable;
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

            return il.Call(_staticCompareMethod);
        }

        public ILEmitter Emit(ILEmitter il) => Emit(il, default).Return();
    }
}
