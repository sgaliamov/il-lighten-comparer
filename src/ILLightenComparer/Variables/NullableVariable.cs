using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Variables
{
    internal sealed class NullableVariable : IVariable
    {
        private readonly MethodInfo _getValueMethod;
        private readonly Dictionary<ushort, LocalBuilder> _nullables;

        public NullableVariable(Type variableType, Type ownerType, LocalBuilder x, LocalBuilder y)
        {
            if (variableType == null) { throw new ArgumentNullException(nameof(variableType)); }

            if (!variableType.IsNullable()) { throw new ArgumentException(nameof(variableType)); }

            OwnerType = ownerType ?? throw new ArgumentNullException(nameof(ownerType));

            VariableType = variableType.GetUnderlyingType();

            _nullables = new Dictionary<ushort, LocalBuilder>(2) {
                { Arg.X, x ?? throw new ArgumentNullException(nameof(x)) },
                { Arg.Y, y ?? throw new ArgumentNullException(nameof(y)) }
            };

            _getValueMethod = variableType.GetPropertyGetter(MethodName.Value);
        }

        // Underlying type of Nullable.
        public Type VariableType { get; }
        public Type OwnerType { get; }

        public ILEmitter Load(ILEmitter il, ushort arg) =>
            il.LoadAddress(_nullables[arg])
              .Call(_getValueMethod);

        public ILEmitter LoadAddress(ILEmitter il, ushort arg)
        {
            var underlyingType = VariableType.GetUnderlyingType();

            return il.LoadAddress(_nullables[arg])
                     .Call(_getValueMethod)
                     .Store(underlyingType, out var x)
                     .LoadAddress(x);
        }
    }
}
