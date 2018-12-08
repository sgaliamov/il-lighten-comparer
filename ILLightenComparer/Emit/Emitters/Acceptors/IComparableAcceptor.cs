using System;

namespace ILLightenComparer.Emit.Emitters.Acceptors
{
    internal interface IComparableAcceptor : IAcceptor
    {
        Type MemberType { get; }
    }
}
