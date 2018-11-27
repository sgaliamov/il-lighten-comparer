using System.Reflection;
using ILLightenComparer.Emit.Emitters.Behavioural;
using ILLightenComparer.Emit.Emitters.Members;

namespace ILLightenComparer.Emit.Members
{
    internal abstract class FieldMember : Member, IFieldMember
    {
        protected FieldMember(FieldInfo fieldInfo)
            : base(fieldInfo.FieldType, fieldInfo.DeclaringType) =>
            FieldInfo = fieldInfo;

        public FieldInfo FieldInfo { get; }
    }
}
