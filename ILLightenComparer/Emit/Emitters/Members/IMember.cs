using System;

namespace ILLightenComparer.Emit.Emitters.Members
{
    internal interface IMember
    {
        Type MemberType { get; }
        Type OwnerType { get; }

        ILEmitter Accept(StackEmitter stacker, ILEmitter il);
        ILEmitter Accept(CompareEmitter emitter, ILEmitter il);
    }
}
