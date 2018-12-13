using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Members
{
    internal interface IFieldMember : IVariable
    {
        FieldInfo FieldInfo { get; }
    }
}
