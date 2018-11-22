using System.Reflection;

namespace ILLightenComparer.Emit.Members
{
    internal abstract class PropertyMember : Member
    {
        private readonly PropertyInfo _propertyInfo;

        protected PropertyMember(PropertyInfo propertyInfo)
            : base(propertyInfo.Name, propertyInfo.PropertyType, propertyInfo.DeclaringType) =>
            _propertyInfo = propertyInfo;

        public MethodInfo GetterMethod => _propertyInfo.GetMethod;
    }
}
