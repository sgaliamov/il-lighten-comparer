using System;
using Illuminator;

namespace ILLightenComparer.Emitters.Variables
{
    internal interface IVariable
    {
        Type OwnerType { get; }
        Type VariableType { get; }

        ILEmitter Load(ILEmitter il, ushort arg);
        ILEmitter LoadAddress(ILEmitter il, ushort arg);
    }
}
