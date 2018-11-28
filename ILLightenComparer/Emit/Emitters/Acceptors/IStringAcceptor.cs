using System;

namespace ILLightenComparer.Emit.Emitters.Acceptors
{
    internal interface IStringAcceptor : IAcceptor
    {
        Type OwnerType { get; }
    }
}
