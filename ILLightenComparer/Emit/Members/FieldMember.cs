using System.Reflection;

namespace ILLightenComparer.Emit.Members
{
    internal abstract class FieldMember : Member
    {
        protected FieldMember(FieldInfo fieldInfo) : base(fieldInfo.DeclaringType) => FieldInfo = fieldInfo;

        public FieldInfo FieldInfo { get; }
    }
}
