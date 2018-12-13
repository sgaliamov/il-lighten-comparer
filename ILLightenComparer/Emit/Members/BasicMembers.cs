using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Emitters.Members;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.Members
{
    internal sealed class BasicFieldMember : FieldMember, IBasicAcceptor, IValueMember
    {
        private BasicFieldMember(FieldInfo fieldInfo) : base(fieldInfo) { }

        public ILEmitter LoadMembers(StackEmitter visitor, Label gotoNext, ILEmitter il)
        {
            return visitor.Visit(this, il, gotoNext);
        }

        public ILEmitter Load(MemberLoader visitor, ILEmitter il, ushort arg)
        {
            return visitor.LoadMember(this, il, arg);
        }

        public ILEmitter LoadAddress(MemberLoader visitor, ILEmitter il, ushort arg)
        {
            return visitor.LoadMemberAddress(this, il, arg);
        }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public ILEmitter Accept(CompareCallVisitor visitor, Label gotoNext, ILEmitter il)
        {
            return visitor.Visit(this, il, gotoNext);
        }

        public static BasicFieldMember Create(MemberInfo memberInfo)
        {
            return memberInfo is FieldInfo info
                   && info
                      .FieldType
                      .GetUnderlyingType()
                      .IsPrimitive()
                       ? new BasicFieldMember(info)
                       : null;
        }
    }

    internal sealed class BasicPropertyMember : PropertyMember, IBasicAcceptor, IValueMember
    {
        private BasicPropertyMember(PropertyInfo propertyInfo) : base(propertyInfo) { }

        public ILEmitter LoadMembers(StackEmitter visitor, Label gotoNext, ILEmitter il)
        {
            return visitor.Visit(this, il, gotoNext);
        }

        public ILEmitter Load(MemberLoader visitor, ILEmitter il, ushort arg)
        {
            return visitor.LoadMember(this, il, arg);
        }

        public ILEmitter LoadAddress(MemberLoader visitor, ILEmitter il, ushort arg)
        {
            return visitor.LoadMemberAddress(this, il, arg);
        }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public ILEmitter Accept(CompareCallVisitor visitor, Label gotoNext, ILEmitter il)
        {
            return visitor.Visit(this, il, gotoNext);
        }

        public static BasicPropertyMember Create(MemberInfo memberInfo)
        {
            return memberInfo is PropertyInfo info
                   && info
                      .PropertyType
                      .GetUnderlyingType()
                      .IsPrimitive()
                       ? new BasicPropertyMember(info)
                       : null;
        }
    }
}
