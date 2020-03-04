using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Emitters.Variables
{
    internal sealed class NullableVariable : IVariable
    {
        public NullableVariable(Type variableType, Type ownerType, LocalBuilder x, LocalBuilder y)
        {
            if (variableType == null) { throw new ArgumentNullException(nameof(variableType)); }

            if (!variableType.IsNullable()) { throw new ArgumentException(nameof(variableType)); }

            OwnerType = ownerType ?? throw new ArgumentNullException(nameof(ownerType));

            VariableType = variableType.GetUnderlyingType();

            Nullables = new Dictionary<ushort, LocalBuilder>(2) {
                { Arg.X, x ?? throw new ArgumentNullException(nameof(x)) },
                { Arg.Y, y ?? throw new ArgumentNullException(nameof(y)) }
            };

            GetValueMethod = variableType.GetPropertyGetter(MethodName.Value);
        }

        public MethodInfo GetValueMethod { get; set; }
        public Dictionary<ushort, LocalBuilder> Nullables { get; }
        public Type OwnerType { get; }

        /// <summary>
        ///     Underlying type of Nullable.
        /// </summary>
        public Type VariableType { get; }

        public ILEmitter Load(ILEmitter il, ushort arg) =>
            il.LoadAddress(Nullables[arg])
              .Call(GetValueMethod);

        public ILEmitter LoadAddress(ILEmitter il, ushort arg)
        {
            var underlyingType = VariableType.GetUnderlyingType();

            return il.LoadAddress(Nullables[arg])
                     .Call(GetValueMethod)
                     .Store(underlyingType, out var x)
                     .LoadAddress(x);
        }
    }
}
