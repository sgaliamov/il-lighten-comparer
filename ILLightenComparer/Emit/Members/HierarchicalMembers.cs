using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Emitters.Members;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.Members
{
    internal sealed class HierarchicalFieldMember : FieldMember, IHierarchicalAcceptor, IArgumentsMember
    {
        private HierarchicalFieldMember(FieldInfo fieldInfo) : base(fieldInfo) { }

        public bool LoadContext => true;

        public ILEmitter LoadMembers(StackEmitter visitor, Label gotoNextMember, ILEmitter il)
        {
            return visitor.Visit(this, il, gotoNextMember);
        }

        public ILEmitter LoadMember(MemberLoader visitor, ILEmitter il, ushort arg)
        {
            return visitor.LoadMember(this, il, arg);
        }

        public ILEmitter LoadMemberAddress(MemberLoader visitor, ILEmitter il, ushort arg)
        {
            return visitor.LoadMemberAddress(this, il, arg);
        }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public static HierarchicalFieldMember Create(MemberInfo memberInfo)
        {
            return memberInfo is FieldInfo info
                   && !info
                       .FieldType
                       .GetUnderlyingType()
                       .IsPrimitive()
                       ? new HierarchicalFieldMember(info)
                       : null;
        }
    }

    internal sealed class HierarchicalPropertyMember : PropertyMember, IHierarchicalAcceptor, IArgumentsMember
    {
        private HierarchicalPropertyMember(PropertyInfo propertyInfo) : base(propertyInfo) { }

        public bool LoadContext => true;

        public ILEmitter LoadMembers(StackEmitter visitor, Label gotoNextMember, ILEmitter il)
        {
            return visitor.Visit(this, il, gotoNextMember);
        }

        public ILEmitter LoadMember(MemberLoader visitor, ILEmitter il, ushort arg)
        {
            return visitor.LoadMember(this, il, arg);
        }

        public ILEmitter LoadMemberAddress(MemberLoader visitor, ILEmitter il, ushort arg)
        {
            return visitor.LoadMemberAddress(this, il, arg);
        }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public static HierarchicalPropertyMember Create(MemberInfo memberInfo)
        {
            return memberInfo is PropertyInfo info
                   && !info
                       .PropertyType
                       .GetUnderlyingType()
                       .IsPrimitive()
                       ? new HierarchicalPropertyMember(info)
                       : null;
        }
    }
}
