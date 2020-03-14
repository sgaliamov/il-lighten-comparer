using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Variables;
using ILLightenComparer.Reflection;
using Illuminator;

namespace ILLightenComparer.Shared.Comparisons
{
    internal sealed class CustomComparison : IStepEmitter
    {
        private readonly IVariable _variable;
        private readonly MethodInfo _delayedCompare;

        public CustomComparison(IVariable variable)
        {
            _variable = variable;
            _delayedCompare = Method.DelayedCompare.MakeGenericMethod(_variable.VariableType);
        }

        public bool PutsResultInStack { get; } = true;

        public ILEmitter Emit(ILEmitter il, Label _)
        {
            il.LoadArgument(Arg.Context);
            _variable.Load(il, Arg.X);
            _variable.Load(il, Arg.Y);

            return il.LoadArgument(Arg.SetX)
                     .LoadArgument(Arg.SetY)
                     .Call(_delayedCompare);
        }

        public ILEmitter Emit(ILEmitter il) => Emit(il, default).Return();
    }
}
