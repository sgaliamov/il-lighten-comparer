using System.Reflection;
using ILLightenComparer.Emit.Emitters.Members;

namespace ILLightenComparer.Emit.Emitters.Behavioural
{
    internal interface IFieldMember : IMember
    {
        FieldInfo FieldInfo { get; }
    }
}