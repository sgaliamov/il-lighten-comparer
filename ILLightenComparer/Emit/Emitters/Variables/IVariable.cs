using System;

namespace ILLightenComparer.Emit.Emitters.Variables
{
    internal interface IVariable
    {
        Type DeclaringType { get; }
        Type VariableType { get; }
    }
}
