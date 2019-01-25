using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Visitors;

namespace ILLightenComparer.Emit.v2.Variables
{
    internal sealed class LocalVariable : IVariable
    {
        public LocalVariable(Type variableType, LocalBuilder x, LocalBuilder y)
        {
            VariableType = variableType ?? throw new ArgumentNullException(nameof(variableType));
            Locals = new Dictionary<ushort, LocalBuilder>(2)
            {
                { Arg.X, x ?? throw new ArgumentNullException(nameof(x)) },
                { Arg.Y, y ?? throw new ArgumentNullException(nameof(y)) }
            };
        }

        public Dictionary<ushort, LocalBuilder> Locals { get; }
        public Type VariableType { get; }

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
