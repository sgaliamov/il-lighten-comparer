using System.Reflection.Emit;

namespace ILLightenComparer.Emit.Emitters.Acceptors
{
    internal interface IAcceptor
    {
        ILEmitter LoadMembers(StackEmitter visitor, Label gotoNextMember, ILEmitter il);
        ILEmitter LoadMember(MemberLoader visitor, ushort arg, ILEmitter il);
        ILEmitter LoadMemberAddress(MemberLoader visitor, ushort arg, ILEmitter il);
        ILEmitter Accept(CompareEmitter visitor, ILEmitter il);
    }
}
