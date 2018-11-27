using System;

namespace ILLightenComparer.Emit.Emitters.Acceptors
{
    internal interface IHierarchicalAcceptor : IAcceptor
    {
        Type MemberType { get; }
    }
}
