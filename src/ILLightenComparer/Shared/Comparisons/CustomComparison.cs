using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Variables;
using Illuminator;
using static Illuminator.Functional;

namespace ILLightenComparer.Shared.Comparisons
{
    internal sealed class CustomComparison : IComparisonEmitter
    {
        private readonly IVariable _variable;
        private readonly MethodInfo _delayedCompare;

        public CustomComparison(IVariable variable, MethodInfo compareMethod)
        {
            _variable = variable;
            _delayedCompare = compareMethod.MakeGenericMethod(_variable.VariableType);
        }

        public bool PutsResultInStack { get; } = true;

        public ILEmitter Emit(ILEmitter il, Label _) => il.Call(
            _delayedCompare,
            LoadArgument(Arg.Context),
            _variable.Load(Arg.X),
            _variable.Load(Arg.Y),
            LoadArgument(Arg.SetX),
            LoadArgument(Arg.SetY));

        public ILEmitter Emit(ILEmitter il) => Emit(il, default).Return();
    }
}
