using System;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Visitors;

namespace ILLightenComparer.Emit.v2.Variables
{
    internal interface IVariable
    {
        Type VariableType { get; }
        Type OwnerType { get; }

        ILEmitter Load(VariableLoader visitor, ILEmitter il, ushort arg);
        ILEmitter LoadAddress(VariableLoader visitor, ILEmitter il, ushort arg);
    }
}
