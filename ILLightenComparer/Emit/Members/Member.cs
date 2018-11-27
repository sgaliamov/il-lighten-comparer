using System;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Members;

namespace ILLightenComparer.Emit.Members
{
    internal abstract class Member : IMember
    {
        protected Member(Type memberType, Type ownerType)
        {
            OwnerType = ownerType;
            MemberType = memberType;
        }

        public Type OwnerType { get; }
        public Type MemberType { get; }

        public abstract ILEmitter Accept(StackEmitter visitor, ILEmitter il);
        public abstract ILEmitter Accept(CompareEmitter visitor, ILEmitter il);
    }
}
