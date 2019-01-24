using System;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Visitors;

namespace ILLightenComparer.Emit.v2.Variables
{
    internal sealed class SealedComparableVariable : IVariable
    {
        public SealedComparableVariable(Type variableType)
        {
            VariableType = variableType;
        }

        public Type VariableType { get; }

        public ILEmitter Load(VariableLoader visitor, ILEmitter il, ushort arg)
        {
            throw new NotImplementedException();
        }

        public ILEmitter LoadAddress(VariableLoader visitor, ILEmitter il, ushort arg)
        {
            throw new NotImplementedException();
        }
    }
}
