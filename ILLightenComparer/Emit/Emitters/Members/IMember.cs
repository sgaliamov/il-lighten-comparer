using System;

namespace ILLightenComparer.Emit.Emitters.Members
{
    internal interface IMember
    {
        Type MemberType { get; }
        Type OwnerType { get; }

        void Accept(StackEmitter stacker, ILEmitter il);
        void Accept(CompareEmitter emitter, ILEmitter il);
    }
}
