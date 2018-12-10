using System;
using System.Reflection;
using ILLightenComparer.Emit.Emitters.Members;

namespace ILLightenComparer.Emit.Members
{
    internal abstract class PropertyMember : IPropertyMember
    {
        private readonly PropertyInfo _propertyInfo;

        protected PropertyMember(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        public Type MemberType => _propertyInfo.PropertyType;
        public MethodInfo GetterMethod => _propertyInfo.GetMethod;
        public Type DeclaringType => _propertyInfo.DeclaringType;
    }

    internal abstract class FieldMember : IFieldMember
    {
        protected FieldMember(FieldInfo fieldInfo)
        {
            FieldInfo = fieldInfo;
        }

        public Type MemberType => FieldInfo.FieldType;
        public FieldInfo FieldInfo { get; }
        public Type DeclaringType => FieldInfo.DeclaringType;
    }
}
