using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Members
{
    internal interface IFieldMember : IMember
    {
        FieldInfo FieldInfo { get; }
    }
}