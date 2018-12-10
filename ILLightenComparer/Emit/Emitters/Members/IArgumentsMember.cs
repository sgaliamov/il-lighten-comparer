using System;
using ILLightenComparer.Emit.Emitters.Acceptors;

namespace ILLightenComparer.Emit.Emitters.Members
{
    internal interface IArgumentsMember : IAcceptor
    {
        Type MemberType { get; }
        bool LoadContext { get; }
    }
}
