using System;
using System.Reflection;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Visitors;

namespace ILLightenComparer.Emit.v2.Variables
{
    internal sealed class PropertyMemberVariable : IVariable
    {
        private readonly PropertyInfo _propertyInfo;

        private PropertyMemberVariable(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        public MethodInfo GetterMethod => _propertyInfo.GetMethod;
        public Type OwnerType => _propertyInfo.DeclaringType;
        public Type VariableType => _propertyInfo.PropertyType;

        public ILEmitter Load(VariableLoader visitor, ILEmitter il, ushort arg)
        {
            return visitor.Load(this, il, arg);
        }

        public ILEmitter LoadAddress(VariableLoader visitor, ILEmitter il, ushort arg)
        {
            return visitor.LoadAddress(this, il, arg);
        }

        public static IVariable Create(MemberInfo memberInfo)
        {
            return memberInfo is PropertyInfo info
                       ? new PropertyMemberVariable(info)
                       : null;
        }
    }
}
