using System.Reflection;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Emitters.Members;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Members
{
    internal sealed class NullableFieldMember : FieldMember, INullableAcceptor, ITwoArgumentsField
    {
        private NullableFieldMember(FieldInfo fieldInfo) : base(fieldInfo)
        {
            var fieldType = fieldInfo.FieldType;
            HasValueMethod = fieldType.GetPropertyGetter(MethodName.HasValue);
            GetValueMethod = fieldType.GetPropertyGetter(MethodName.Value);
        }

        public MethodInfo GetValueMethod { get; }
        public MethodInfo HasValueMethod { get; }

        public ILEmitter LoadMembers(StackEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

        public static NullableFieldMember Create(MemberInfo memberInfo) =>
            memberInfo is FieldInfo info && info.FieldType.IsNullable()
                ? new NullableFieldMember(info)
                : null;
    }

    internal sealed class NullablePropertyMember : PropertyMember, INullableAcceptor, ITwoArgumentsProperty
    {
        private NullablePropertyMember(PropertyInfo propertyInfo) : base(propertyInfo)
        {
            var propertyType = propertyInfo.PropertyType;
            HasValueMethod = propertyType.GetPropertyGetter(MethodName.HasValue);
            GetValueMethod = propertyType.GetPropertyGetter(MethodName.Value);
        }

        public MethodInfo GetValueMethod { get; }
        public MethodInfo HasValueMethod { get; }

        public ILEmitter LoadMembers(StackEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

        public static NullablePropertyMember Create(MemberInfo memberInfo) =>
            memberInfo is PropertyInfo info && info.PropertyType.IsNullable()
                ? new NullablePropertyMember(info)
                : null;
    }
}
