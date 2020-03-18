using System.Reflection;
using ILLightenComparer.Shared;
using ILLightenComparer.Variables;
using Illuminator;
using static Illuminator.Functional;

namespace ILLightenComparer.Equality.Hashers
{
    internal sealed class CustomHasher : IHasherEmitter
    {
        private readonly IVariable _variable;
        private readonly MethodInfo _delayedHash;

        public CustomHasher(IVariable variable, MethodInfo delayedHash)
        {
            _variable = variable;
            _delayedHash = delayedHash;
        }

        public ILEmitter Emit(ILEmitter il) => il.Call(
            _delayedHash,
            LoadArgument(Arg.Context),
            _variable.Load(Arg.X),
            LoadArgument(Arg.SetX));
    }
}
