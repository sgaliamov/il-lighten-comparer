using System;

namespace ILLightenComparer.Emit.Emitters.Members
{
    internal interface IMember
    {
        Type DeclaringType { get; }
        Type MemberType { get; }
    }
}
