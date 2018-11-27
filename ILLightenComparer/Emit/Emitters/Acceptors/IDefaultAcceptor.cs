using System;

namespace ILLightenComparer.Emit.Emitters.Acceptors
{
    internal interface IDefaultAcceptor : IAcceptor
    {
        Type MemberType { get; }
    }
}
