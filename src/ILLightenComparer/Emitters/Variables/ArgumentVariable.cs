using System;
using ILLightenComparer.Emitters.Visitors;
using Illuminator;

namespace ILLightenComparer.Emitters.Variables
{
    internal sealed class ArgumentVariable : IVariable
    {
        public ArgumentVariable(Type variableType)
        {
            OwnerType = variableType;
            VariableType = variableType;
        }

        public Type OwnerType { get; }
        public Type VariableType { get; }

        public ILEmitter Load(VariableLoader visitor, ILEmitter il, ushort arg) => visitor.Load(il, arg);

        public ILEmitter LoadAddress(VariableLoader visitor, ILEmitter il, ushort arg) => visitor.LoadAddress(il, arg);
    }
}
