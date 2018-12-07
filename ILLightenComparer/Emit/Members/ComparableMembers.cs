using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Emitters.Members;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.Members
{
    internal sealed class ComparableFieldMember : FieldMember, IComparableAcceptor, IArgumentsField
    {
        private ComparableFieldMember(FieldInfo fieldInfo) : base(fieldInfo) { }

        public bool LoadContext => false;

        public ILEmitter LoadMembers(StackEmitter visitor, Label gotoNextMember, ILEmitter il) =>
            visitor.Visit(this, il, gotoNextMember);

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

        public static ComparableFieldMember Create(MemberInfo memberInfo) =>
            memberInfo is FieldInfo info
            && info
               .FieldType
               .GetUnderlyingType()
               .ImplementsGeneric(typeof(IComparable<>), info.FieldType)
                ? new ComparableFieldMember(info)
                : null;
    }

    internal sealed class ComparablePropertyMember : PropertyMember, IComparableAcceptor, IArgumentsProperty
    {
        private ComparablePropertyMember(PropertyInfo propertyInfo) : base(propertyInfo) { }

        public bool LoadContext => false;

        public ILEmitter LoadMembers(StackEmitter visitor, Label gotoNextMember, ILEmitter il) =>
            visitor.Visit(this, il, gotoNextMember);

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

        public static ComparablePropertyMember Create(MemberInfo memberInfo) =>
            memberInfo is PropertyInfo info
            && info
               .PropertyType
               .GetUnderlyingType()
               .ImplementsGeneric(typeof(IComparable<>), info.PropertyType)
                ? new ComparablePropertyMember(info)
                : null;
    }
}
