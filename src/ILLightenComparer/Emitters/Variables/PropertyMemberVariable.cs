using System;
using System.Reflection;
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

        public ILEmitter Load(ILEmitter il, ushort arg)
        {
            if (OwnerType.IsValueType) {
                il.LoadArgumentAddress(arg);
            } else {
                il.LoadArgument(arg);
            }

            return il.Call(GetterMethod);
        }

        public ILEmitter LoadAddress(ILEmitter il, ushort arg)
        {
            if (OwnerType.IsValueType) {
                il.LoadArgumentAddress(arg);
            } else {
                il.LoadArgument(arg);
            }

            return il.Call(GetterMethod);
        }

        public static IVariable Create(MemberInfo memberInfo) =>
            memberInfo is PropertyInfo info
                ? new PropertyMemberVariable(info)
                : null;
    }
}
