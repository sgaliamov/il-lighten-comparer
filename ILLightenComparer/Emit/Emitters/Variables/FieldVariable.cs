using System;
using System.Reflection;
using ILLightenComparer.Emit.Emitters.Visitors;
using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit.Emitters.Variables
{
    internal sealed class FieldVariable : IVariable
    {
        private FieldVariable(FieldInfo fieldInfo)
        {
            FieldInfo = fieldInfo;
        }

        public FieldInfo FieldInfo { get; }
        public Type OwnerType => FieldInfo.DeclaringType;
        public Type VariableType => FieldInfo.FieldType;

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
            return memberInfo is FieldInfo info
                       ? new FieldVariable(info)
                       : null;
        }
    }
}
