using System;

namespace ILLightenComparer.Emit.Emitters.Members
{
    internal interface IVariable
    {
        Type DeclaringType { get; }
        Type VariableType { get; }
    }
}
