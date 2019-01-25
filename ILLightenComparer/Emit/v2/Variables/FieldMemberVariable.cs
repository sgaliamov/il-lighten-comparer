using System;
using System.Reflection;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Visitors;

namespace ILLightenComparer.Emit.v2.Variables
{
    internal sealed class FieldMemberVariable : IVariable
    {
        private FieldMemberVariable(FieldInfo fieldInfo)
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
                       ? new FieldMemberVariable(info)
                       : null;
        }
    }
}
