using System;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Members;

namespace ILLightenComparer.Emit.Members
{
    internal abstract class Member : IMember
    {
        protected Member(Type ownerType) => OwnerType = ownerType;

        public Type OwnerType { get; }

        public abstract void Accept(StackEmitter visitor, ILEmitter il);
        public abstract void Accept(CompareEmitter visitor, ILEmitter il);
    }
}
