using System;
using ILLightenComparer.Emitters.Visitors;
using Illuminator;

namespace ILLightenComparer.Emitters.Variables
{
    internal interface IVariable
    {
        Type OwnerType { get; }
        Type VariableType { get; }

        ILEmitter Load(VariableLoader visitor, ILEmitter il, ushort arg);
        ILEmitter LoadAddress(VariableLoader visitor, ILEmitter il, ushort arg);
    }
}
