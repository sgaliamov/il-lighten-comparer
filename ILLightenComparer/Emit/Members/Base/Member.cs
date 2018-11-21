using System;
using ILLightenComparer.Emit.Visitors;

namespace ILLightenComparer.Emit.Members.Base
{
    internal abstract class Member
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

        public abstract void Accept(IVisitor visitor, ILEmitter il);
    }
}
