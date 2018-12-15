using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Variables
{
    internal interface IFieldVariable : IVariable
    {
        FieldInfo FieldInfo { get; }
    }
}
