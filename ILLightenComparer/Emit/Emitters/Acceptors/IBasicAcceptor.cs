using System;

namespace ILLightenComparer.Emit.Emitters.Acceptors
{
    internal interface IBasicAcceptor : IAcceptor
    {
        Type MemberType { get; }
    }
}
