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

        public ILEmitter LoadMembers(StackEmitter visitor, Label gotoNextMember, ILEmitter il) =>
            visitor.Visit(this, il, gotoNextMember);

        public ILEmitter LoadMember(MemberLoader visitor, ILEmitter il, ushort arg) =>
            visitor.LoadMember(this, il, arg);

        public ILEmitter LoadMemberAddress(MemberLoader visitor, ILEmitter il, ushort arg) =>
            visitor.LoadMemberAddress(this, il, arg);

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

        public static IntegralFieldMember Create(MemberInfo memberInfo) =>
            memberInfo is FieldInfo info
            && info
               .FieldType
               .GetUnderlyingType()
               .IsSmallIntegral()
                ? new IntegralFieldMember(info)
                : null;
    }

    internal sealed class IntegralPropertyMember : PropertyMember, IIntegralAcceptor, IArgumentsMember
    {
        private IntegralPropertyMember(PropertyInfo propertyInfo) : base(propertyInfo) { }

        public bool LoadContext => false;

        public ILEmitter LoadMembers(StackEmitter visitor, Label gotoNextMember, ILEmitter il) => visitor.Visit(this, il, gotoNextMember);

        public ILEmitter LoadMember(MemberLoader visitor, ILEmitter il, ushort arg) =>
            visitor.LoadMember(this, il, arg);

        public ILEmitter LoadMemberAddress(MemberLoader visitor, ILEmitter il, ushort arg) =>
            visitor.LoadMemberAddress(this, il, arg);

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

        public static IntegralPropertyMember Create(MemberInfo memberInfo) =>
            memberInfo is PropertyInfo info
            && info
               .PropertyType
               .GetUnderlyingType()
               .IsSmallIntegral()
                ? new IntegralPropertyMember(info)
                : null;
    }
}
