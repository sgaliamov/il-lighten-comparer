using System;
using System.Reflection;
using ILLightenComparer.Emitters.Visitors;
using Illuminator;

namespace ILLightenComparer.Emitters.Variables
{
    internal sealed class PropertyMemberVariable : IVariable
    {
        private readonly PropertyInfo _propertyInfo;

        private PropertyMemberVariable(PropertyInfo propertyInfo) => _propertyInfo = propertyInfo;

        public MethodInfo GetterMethod => _propertyInfo.GetMethod;
        public Type OwnerType => _propertyInfo.DeclaringType;
        public Type VariableType => _propertyInfo.PropertyType;

        public ILEmitter Load(VariableLoader visitor, ILEmitter il, ushort arg) => visitor.Load(this, il, arg);

        public ILEmitter LoadAddress(VariableLoader visitor, ILEmitter il, ushort arg) => visitor.LoadAddress(this, il, arg);

        public static IVariable Create(MemberInfo memberInfo) =>
            memberInfo is PropertyInfo info
                ? new PropertyMemberVariable(info)
                : null;
    }
}
