using System;

namespace ILLightenComparer.Emit.Emitters.Members
{
    internal interface IMember
    {
        Type MemberType { get; }

        void Accept(StackEmitter stacker, ILEmitter il);
        void Accept(CompareEmitter emitter, ILEmitter il);
    }
}
