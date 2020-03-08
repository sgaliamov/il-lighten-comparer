using System;
using Illuminator;

namespace ILLightenComparer.Variables
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

        public ILEmitter Load(ILEmitter il, ushort arg) => il.LoadArgument(arg);
        public ILEmitter LoadAddress(ILEmitter il, ushort arg) => il.LoadArgumentAddress(arg);
    }
}
