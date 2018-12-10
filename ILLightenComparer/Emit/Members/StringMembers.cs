using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Emitters.Members;

namespace ILLightenComparer.Emit.Members
{
    internal sealed class StringFieldMember : FieldMember, IStringAcceptor, IArgumentsMember
    {
        private StringFieldMember(FieldInfo fieldInfo) : base(fieldInfo) { }

        public bool LoadContext => false;

        public ILEmitter LoadMembers(StackEmitter visitor, Label gotoNextMember, ILEmitter il) =>
            visitor.Visit(this, il, gotoNextMember);
        
        public ILEmitter LoadMember(MemberLoader visitor, ILEmitter il, ushort arg) => 
            visitor.LoadMember(this, il, arg);
        
        public ILEmitter LoadMemberAddress(MemberLoader visitor, ILEmitter il, ushort arg) =>
            visitor.LoadMemberAddress(this, il, arg);

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

        public static StringFieldMember Create(MemberInfo memberInfo) =>
            memberInfo is FieldInfo info
            && info.FieldType == typeof(string)
                ? new StringFieldMember(info)
                : null;
    }

    internal sealed class StringPropertyMember : PropertyMember, IStringAcceptor, IArgumentsMember
    {
        private StringPropertyMember(PropertyInfo propertyInfo) : base(propertyInfo) { }

        public bool LoadContext => false;

        public ILEmitter LoadMembers(StackEmitter visitor, Label gotoNextMember, ILEmitter il) =>
            visitor.Visit(this, il, gotoNextMember);
        
        public ILEmitter LoadMember(MemberLoader visitor, ILEmitter il, ushort arg) => 
            visitor.LoadMember(this, il, arg);
        
        public ILEmitter LoadMemberAddress(MemberLoader visitor, ILEmitter il, ushort arg) =>
            visitor.LoadMemberAddress(this, il, arg);

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

        public static StringPropertyMember Create(MemberInfo memberInfo) =>
            memberInfo is PropertyInfo info
            && info.PropertyType == typeof(string)
                ? new StringPropertyMember(info)
                : null;
    }
}
