using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Emitters.Members;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.Members
{
    internal sealed class IntegralFieldMember : FieldMember, IIntegralAcceptor, IArgumentsMember
    {
        private IntegralFieldMember(FieldInfo fieldInfo) : base(fieldInfo) { }

        public bool LoadContext => false;

        public ILEmitter LoadMembers(StackEmitter visitor, ILEmitter il, Label gotoNext)
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

        public ILEmitter Accept(CompareCallVisitor visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public static IntegralFieldMember Create(MemberInfo memberInfo)
        {
            return memberInfo is FieldInfo info
                   && info
                      .FieldType
                      .GetUnderlyingType()
                      .IsSmallIntegral()
                       ? new IntegralFieldMember(info)
                       : null;
        }
    }

    internal sealed class IntegralPropertyMember : PropertyMember, IIntegralAcceptor, IArgumentsMember
    {
        private IntegralPropertyMember(PropertyInfo propertyInfo) : base(propertyInfo) { }

        public bool LoadContext => false;

        public ILEmitter LoadMembers(StackEmitter visitor, ILEmitter il, Label gotoNext)
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

        public ILEmitter Accept(CompareCallVisitor visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public static IntegralPropertyMember Create(MemberInfo memberInfo)
        {
            return memberInfo is PropertyInfo info
                   && info
                      .PropertyType
                      .GetUnderlyingType()
                      .IsSmallIntegral()
                       ? new IntegralPropertyMember(info)
                       : null;
        }
    }
}
