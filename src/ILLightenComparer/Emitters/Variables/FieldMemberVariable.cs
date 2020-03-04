using System;
using System.Reflection;
using Illuminator;

namespace ILLightenComparer.Emitters.Variables
{
    internal sealed class FieldMemberVariable : IVariable
    {
        private FieldMemberVariable(FieldInfo fieldInfo) => FieldInfo = fieldInfo;

        public FieldInfo FieldInfo { get; }
        public Type OwnerType => FieldInfo.DeclaringType;
        public Type VariableType => FieldInfo.FieldType;

        public ILEmitter Load(ILEmitter il, ushort arg) => il.LoadArgument(arg).LoadField(FieldInfo);

        public ILEmitter LoadAddress(ILEmitter il, ushort arg)
        {
            if (OwnerType.IsValueType) {
                il.LoadArgumentAddress(arg);
            } else {
                il.LoadArgument(arg);
            }

            return il.LoadFieldAddress(FieldInfo);
        }

        public static IVariable Create(MemberInfo memberInfo) =>
            memberInfo is FieldInfo info
                ? new FieldMemberVariable(info)
                : null;
    }
}
