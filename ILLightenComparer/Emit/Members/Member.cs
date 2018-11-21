using System;
using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit.Members
{
    internal abstract class Member
    {
        public Type MemberType { get; set; }
        public string Name { get; set; }
        public Type OwnerType { get; set; }

        public abstract void Accept(IVisitor visitor, ILEmitter il);
    }
}
