using System.Reflection;
using ILLightenComparer.Shared;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Equality.Hashers
{
    internal sealed class PrimitiveHasher : IHasherEmitter
    {
        private readonly IVariable _variable;
        private readonly MethodInfo _getHashMethod;

        private PrimitiveHasher(IVariable variable)
        {
            _variable = variable;
            _getHashMethod = _variable.VariableType.GetMethod(nameof(GetHashCode));
        }

        public static PrimitiveHasher Create(IVariable variable)
        {
            if (variable.VariableType.IsPrimitive()) {
                return new PrimitiveHasher(variable);
            }

            return null;
        }

        public ILEmitter Emit(ILEmitter il) => il.Call(_getHashMethod, _variable.Load(Arg.X));
    }
}
