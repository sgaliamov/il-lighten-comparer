using System;
using ILLightenComparer.Emit.Emitters.Acceptors;

namespace ILLightenComparer.Emit.Emitters.Members
{
    internal interface IValueMember : IAcceptor
    {
        Type MemberType { get; }
    }
}
