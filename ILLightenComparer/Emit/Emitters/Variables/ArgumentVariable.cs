using System;
using ILLightenComparer.Emit.Emitters.Visitors;

namespace ILLightenComparer.Emit.Emitters.Variables
{
    internal sealed class ArgumentVariable : IVariable
    {
        public ArgumentVariable(Type ownerType, Type variableType)
        {
            OwnerType = ownerType;
            VariableType = variableType;
        }

        public Type OwnerType { get; }
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
