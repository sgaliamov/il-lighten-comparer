using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
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
            _getHashMethod = _variable.VariableType.GetUnderlyingType().GetMethod(nameof(GetHashCode));
        }

        public static PrimitiveHasher Create(IVariable variable)
        {
            var variableType = variable.VariableType;
            if (variable.VariableType.IsPrimitive() && !variableType.IsClass) {
                return new PrimitiveHasher(variable);
            }

            return null;
        }

        public ILEmitter Emit(ILEmitter il) => il.Call(_getHashMethod, _variable.LoadAddress(Arg.Input));

        public ILEmitter Emit(ILEmitter il, LocalBuilder _) => Emit(il);
    }
}
