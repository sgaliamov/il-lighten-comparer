using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Extensions;
using ILLightenComparer.Shared;
using ILLightenComparer.Variables;
using Illuminator;
using static Illuminator.Functional;

namespace ILLightenComparer.Equality.Comparisons
{
    internal sealed class CustomHasher : IStepEmitter
    {
        private readonly IVariable _variable;
        private readonly MethodInfo _delayedHash;

        public CustomHasher(IVariable variable, MethodInfo delayedHash)
        {
            _variable = variable;
            _delayedHash = delayedHash;
        }

        public bool PutsResultInStack { get; } = true;

        public ILEmitter Emit(ILEmitter il, Label _) => il.Call(
            _delayedHash,
            LoadArgument(Arg.Context),
            _variable.Load(Arg.X),
            LoadArgument(Arg.SetX));

        public ILEmitter Emit(ILEmitter il) => Emit(il, default).Return();
    }
}
