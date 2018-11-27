using System;

namespace ILLightenComparer.Emit.Emitters.Acceptors
{
    internal interface IAcceptor
    {
        Type MemberType { get; }

        ILEmitter Accept(StackEmitter stacker, ILEmitter il);
        ILEmitter Accept(CompareEmitter emitter, ILEmitter il);
    }
}
