using System;
using System.Reflection;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Emitters.Members;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.Members
{
    internal sealed class ComparableFieldMember : FieldMember, IComparableAcceptor, ITwoArgumentsField
    {
        private ComparableFieldMember(FieldInfo fieldInfo) : base(fieldInfo) { }

        public ILEmitter LoadMembers(StackEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

        public static ComparableFieldMember Create(MemberInfo memberInfo) =>
            memberInfo is FieldInfo info && info.FieldType.ImplementsGeneric(typeof(IComparable<>), info.FieldType)
                ? new ComparableFieldMember(info)
                : null;
    }

    internal sealed class ComparablePropertyMember : PropertyMember, IComparableAcceptor, ITwoArgumentsProperty
    {
        private ComparablePropertyMember(PropertyInfo propertyInfo) : base(propertyInfo) { }

        public ILEmitter LoadMembers(StackEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

        public static ComparablePropertyMember Create(MemberInfo memberInfo)
        {
            if (!(memberInfo is PropertyInfo info))
            {
                return null;
            }

            var isComparable = info.PropertyType.ImplementsGeneric(typeof(IComparable<>), info.PropertyType);

            return isComparable
                ? new ComparablePropertyMember(info)
                : null;
        }
    }
}
