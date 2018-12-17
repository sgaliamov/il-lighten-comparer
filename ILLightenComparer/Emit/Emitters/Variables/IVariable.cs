using System;
using ILLightenComparer.Emit.Emitters.Visitors;

namespace ILLightenComparer.Emit.Emitters.Variables
{
    internal interface IVariable
    {
        Type OwnerType { get; }
        Type VariableType { get; }

        ILEmitter Load(VariableLoader visitor, ILEmitter il, ushort arg);
        ILEmitter LoadAddress(VariableLoader visitor, ILEmitter il, ushort arg);
    }
}
