using System.Reflection;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Emitters.Members;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.Members
{
    internal sealed class IntegralFieldMember : FieldMember, ITwoArgumentsField, IIntegralAcceptor
    {
        private IntegralFieldMember(FieldInfo fieldInfo) : base(fieldInfo) { }

        public ILEmitter LoadMembers(StackEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

        public static IntegralFieldMember Create(MemberInfo memberInfo) =>
            memberInfo is FieldInfo info && info.FieldType.IsSmallIntegral()
                ? new IntegralFieldMember(info)
                : null;
    }

    internal sealed class IntegralPropertyMember : PropertyMember, ITwoArgumentsProperty, IIntegralAcceptor
    {
        private IntegralPropertyMember(PropertyInfo propertyInfo) : base(propertyInfo) { }

        public ILEmitter LoadMembers(StackEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

        public static IntegralPropertyMember Create(MemberInfo memberInfo) =>
            memberInfo is PropertyInfo info && info.PropertyType.IsSmallIntegral()
                ? new IntegralPropertyMember(info)
                : null;
    }
}
