using System.Reflection;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Emitters.Members;

namespace ILLightenComparer.Emit.Members
{
    internal sealed class HierarchicalFieldMember : FieldMember, IHierarchicalAcceptor, ITwoArgumentsField
    {
        private HierarchicalFieldMember(FieldInfo fieldInfo) : base(fieldInfo) { }

        public ILEmitter LoadMembers(StackEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

        public static HierarchicalFieldMember Create(MemberInfo memberInfo) =>
            memberInfo is FieldInfo info
                ? new HierarchicalFieldMember(info)
                : null;
    }

    internal sealed class HierarchicalPropertyMember : PropertyMember, IHierarchicalAcceptor, ITwoArgumentsProperty
    {
        private HierarchicalPropertyMember(PropertyInfo propertyInfo) : base(propertyInfo) { }

        public ILEmitter LoadMembers(StackEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

        public static HierarchicalPropertyMember Create(MemberInfo memberInfo) =>
            memberInfo is PropertyInfo info
                ? new HierarchicalPropertyMember(info)
                : null;
    }
}
