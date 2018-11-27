using System;

namespace ILLightenComparer.Emit.Emitters.Members
{
    internal interface ICallableProperty : IPropertyMember
    {
        Type MemberType { get; }
    }
}
