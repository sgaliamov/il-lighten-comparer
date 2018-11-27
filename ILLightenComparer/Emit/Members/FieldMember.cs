using System;
using System.Reflection;
using ILLightenComparer.Emit.Emitters.Members;

namespace ILLightenComparer.Emit.Members
{
    internal abstract class FieldMember : IFieldMember
    {
        protected FieldMember(FieldInfo fieldInfo) =>
            FieldInfo = fieldInfo;

        public Type MemberType => FieldInfo.FieldType;
        public FieldInfo FieldInfo { get; }
        public Type OwnerType => FieldInfo.DeclaringType;
    }
}
