using System;

namespace ILLightenComparer.Emit.Emitters.Acceptors
{
    internal interface INestedAcceptor : IAcceptor
    {
        Type MemberType { get; }
    }
}
