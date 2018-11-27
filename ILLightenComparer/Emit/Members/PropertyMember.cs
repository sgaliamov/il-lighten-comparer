using System;
using System.Reflection;
using ILLightenComparer.Emit.Emitters.Members;

namespace ILLightenComparer.Emit.Members
{
    internal abstract class PropertyMember : IPropertyMember
    {
        private readonly PropertyInfo _propertyInfo;

        protected PropertyMember(PropertyInfo propertyInfo) => _propertyInfo = propertyInfo;

        public Type MemberType => _propertyInfo.PropertyType;
        public MethodInfo GetterMethod => _propertyInfo.GetMethod;
        public Type OwnerType => _propertyInfo.DeclaringType;
    }
}
