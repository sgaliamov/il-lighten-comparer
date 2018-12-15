using System;
using System.Reflection;
using ILLightenComparer.Emit.Emitters.Members;

namespace ILLightenComparer.Emit.Members
{
    internal abstract class PropertyVariable : IPropertyVariable
    {
        private readonly PropertyInfo _propertyInfo;

        protected PropertyVariable(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        public Type VariableType => _propertyInfo.PropertyType;
        public MethodInfo GetterMethod => _propertyInfo.GetMethod;
        public Type DeclaringType => _propertyInfo.DeclaringType;
    }

    internal abstract class FieldVariable : IFieldVariable
    {
        protected FieldVariable(FieldInfo fieldInfo)
        {
            FieldInfo = fieldInfo;
        }

        public Type VariableType => FieldInfo.FieldType;
        public FieldInfo FieldInfo { get; }
        public Type DeclaringType => FieldInfo.DeclaringType;
    }
}
