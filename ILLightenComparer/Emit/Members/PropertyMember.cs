using System.Reflection;
using ILLightenComparer.Emit.Emitters.Members;

namespace ILLightenComparer.Emit.Members
{
    internal abstract class PropertyMember : Member, IPropertyMember
    {
        private readonly PropertyInfo _propertyInfo;

        protected PropertyMember(PropertyInfo propertyInfo)
            : base(propertyInfo.PropertyType, propertyInfo.DeclaringType) =>
            _propertyInfo = propertyInfo;

        public MethodInfo GetterMethod => _propertyInfo.GetMethod;
    }
}
