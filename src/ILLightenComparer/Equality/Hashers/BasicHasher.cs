using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Extensions;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Equality.Hashers
{
    internal sealed class BasicHasher : IHasherEmitter
    {
        private readonly IVariable _variable;
        private readonly MethodInfo _getHashMethod;

        private BasicHasher(IVariable variable)
        {
            _variable = variable;
            _getHashMethod = _variable.VariableType.GetUnderlyingType().GetMethod(nameof(GetHashCode));
        }

        public static BasicHasher Create(IVariable variable)
        {
            if (variable.VariableType.GetUnderlyingType().IsBasic()) {
                return new BasicHasher(variable);
            }

            return null;
        }

        public ILEmitter Emit(ILEmitter il) => il.Call(
            _getHashMethod,
            _variable.VariableType.IsValueType ? _variable.LoadAddress(Arg.Input) : _variable.Load(Arg.Input));

        public ILEmitter Emit(ILEmitter il, LocalBuilder _) => Emit(il);
    }
}
