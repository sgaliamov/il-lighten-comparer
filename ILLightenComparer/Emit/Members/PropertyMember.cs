using System.Reflection;
using ILLightenComparer.Emit.Emitters.Members;

namespace ILLightenComparer.Emit.Members
{
    internal abstract class PropertyMember : Member, IPropertyMember
    {
        protected PropertyMember(PropertyInfo propertyInfo)
            : base(propertyInfo.PropertyType, propertyInfo.DeclaringType) =>
            PropertyInfo = propertyInfo;

        public PropertyInfo PropertyInfo { get; }

        public MethodInfo GetterMethod => PropertyInfo.GetMethod;
    }
}
