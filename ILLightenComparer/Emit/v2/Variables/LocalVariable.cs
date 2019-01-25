using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Visitors;

namespace ILLightenComparer.Emit.v2.Variables
{
    internal sealed class LocalVariable : IVariable
    {
        public LocalVariable(Type variableType, Type ownerType, LocalBuilder x, LocalBuilder y)
        {
            VariableType = variableType ?? throw new ArgumentNullException(nameof(variableType));

            OwnerType = ownerType ?? throw new ArgumentNullException(nameof(ownerType));

            Locals = new Dictionary<ushort, LocalBuilder>(2)
            {
                { Arg.X, x ?? throw new ArgumentNullException(nameof(x)) },
                { Arg.Y, y ?? throw new ArgumentNullException(nameof(y)) }
            };
        }

        public Dictionary<ushort, LocalBuilder> Locals { get; }
        public Type VariableType { get; }
        public Type OwnerType { get; }

        public ILEmitter Load(VariableLoader visitor, ILEmitter il, ushort arg)
        {
            return visitor.Load(this, il, arg);
        }

        public ILEmitter LoadAddress(VariableLoader visitor, ILEmitter il, ushort arg)
        {
            return visitor.LoadAddress(this, il, arg);
        }
    }
}
