using System.Reflection;

namespace ILLightenComparer.Emit.Members.Base
{
    internal abstract class FieldMember : Member
    {
        public FieldInfo FieldInfo { get; set; }
    }
}
