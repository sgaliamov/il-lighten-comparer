using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Members
{
    internal interface IFieldVariable : IVariable
    {
        FieldInfo FieldInfo { get; }
    }
}
