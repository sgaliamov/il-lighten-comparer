using System.Reflection.Emit;

namespace ILLightenComparer.Emit.Emitters.Acceptors
{
    internal interface IAcceptor
    {
        ILEmitter LoadMembers(StackEmitter visitor, Label gotoNextMember, ILEmitter il);
        ILEmitter Load(MemberLoader visitor, ILEmitter il, ushort arg);
        ILEmitter LoadAddress(MemberLoader visitor, ILEmitter il, ushort arg);
        ILEmitter Accept(CompareEmitter visitor, ILEmitter il);
    }
}
