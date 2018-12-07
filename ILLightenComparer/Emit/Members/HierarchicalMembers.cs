using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Emitters.Members;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.Members
{
    internal sealed class HierarchicalFieldMember : FieldMember, IHierarchicalAcceptor, ITwoArgumentsField
    {
        private HierarchicalFieldMember(FieldInfo fieldInfo) : base(fieldInfo) { }

        public ILEmitter LoadMembers(StackEmitter visitor, Label gotoNextMember, ILEmitter il) =>
            visitor.Visit(this, il, gotoNextMember);

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

        public static HierarchicalFieldMember Create(MemberInfo memberInfo) =>
            memberInfo is FieldInfo info
            && !info
                .FieldType
                .GetUnderlyingType()
                .IsPrimitive()
                ? new HierarchicalFieldMember(info)
                : null;
    }

    internal sealed class HierarchicalPropertyMember : PropertyMember, IHierarchicalAcceptor, ITwoArgumentsProperty
    {
        private HierarchicalPropertyMember(PropertyInfo propertyInfo) : base(propertyInfo) { }

        public ILEmitter LoadMembers(StackEmitter visitor, Label gotoNextMember, ILEmitter il) =>
            visitor.Visit(this, il, gotoNextMember);

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

        public static HierarchicalPropertyMember Create(MemberInfo memberInfo) =>
            memberInfo is PropertyInfo info
            && !info
                .PropertyType
                .GetUnderlyingType()
                .IsPrimitive()
                ? new HierarchicalPropertyMember(info)
                : null;
    }
}
