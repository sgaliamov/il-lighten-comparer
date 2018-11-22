using System.Reflection;

namespace ILLightenComparer.Emit.Members
{
    internal abstract class FieldMember : Member
    {
        protected FieldMember(FieldInfo fieldInfo) :
            base(fieldInfo.Name, fieldInfo.FieldType, fieldInfo.DeclaringType) =>
            FieldInfo = fieldInfo;

        public FieldInfo FieldInfo { get; }
    }
}
