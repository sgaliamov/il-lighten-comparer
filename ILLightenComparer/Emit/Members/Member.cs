using System;
using ILLightenComparer.Emit.Emitters;

namespace ILLightenComparer.Emit.Members
{
    internal abstract class Member : IMember
    {
        protected Member(string name, Type memberType, Type ownerType)
        {
            Name = name;
            MemberType = memberType;
            OwnerType = ownerType;
        }

        public Type MemberType { get; }
        public string Name { get; }
        public Type OwnerType { get; }

        public bool IsValueType => OwnerType.IsValueType;

        public abstract void Accept(StackEmitter visitor, ILEmitter il);
        public abstract void Accept(CompareEmitter visitor, ILEmitter il);
    }
}
