using System;
using System.Reflection;
using Illuminator;

namespace ILLightenComparer.Emitters.Variables
{
    internal sealed class FieldMemberVariable : IVariable
    {
        private readonly FieldInfo _fieldInfo;
         
        private FieldMemberVariable(FieldInfo fieldInfo) => _fieldInfo = fieldInfo;

        public Type OwnerType => _fieldInfo.DeclaringType;
        public Type VariableType => _fieldInfo.FieldType;

        public ILEmitter Load(ILEmitter il, ushort arg) => il.LoadArgument(arg).LoadField(_fieldInfo);

        public ILEmitter LoadAddress(ILEmitter il, ushort arg)
        {
            if (OwnerType.IsValueType) {
                il.LoadArgumentAddress(arg);
            } else {
                il.LoadArgument(arg);
            }

            return il.LoadFieldAddress(_fieldInfo);
        }

        public static IVariable Create(MemberInfo memberInfo) =>
            memberInfo is FieldInfo info
                ? new FieldMemberVariable(info)
                : null;
    }
}
